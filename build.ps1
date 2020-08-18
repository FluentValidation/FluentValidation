param(
  [string]$configuration = 'Release',
  [string]$path = $PSScriptRoot,
  [string[]]$targets = 'default'
)

$ErrorActionPreference = "Stop"

# Boostrap posh-build
$build_dir = Join-Path $path ".build"
if (! (Test-Path (Join-Path $build_dir "Posh-Build.ps1"))) {
  Write-Host "Installing posh-build..."; New-Item -Type Directory $build_dir -ErrorAction Ignore | Out-Null;
  (New-Object Net.WebClient).DownloadFile('https://raw.githubusercontent.com/jeremyskinner/posh-build/master/Posh-Build.ps1', "$build_dir/Posh-Build.ps1")
}
. (Join-Path $build_dir "Posh-Build.ps1")

# Set these variables as desired
$packages_dir = Join-Path $build_dir "packages"
$output_dir = Join-Path $build_dir $configuration
$solution_file = Join-Path $path "FluentValidation.sln"
$nuget_key = Resolve-Path "~/Dropbox/nuget-access-key.txt" -ErrorAction Ignore

target default -depends compile, test, deploy
target ci -depends install-dotnet-core, default

target compile {
  Invoke-Dotnet clean $solution_file -c $configuration
  Invoke-Dotnet build $solution_file -c $configuration --no-incremental
}

target test {
  Invoke-Dotnet test $solution_file -c $configuration --no-build --logger trx
}

target deploy {
  Invoke-Dotnet pack $solution_file -c $configuration
}

target verify-package {
  if (-not (test-path "$nuget_key")) {
    throw "Could not find the NuGet access key."
  }

  Get-ChildItem $output_dir -Recurse *.dll | ForEach {
    $asm = $_.FullName
    if (! (verify_assembly $asm)) {
      throw "$asm is not signed"
    }
  }
  write-host Package verified
}

target publish -depends verify-package {
  $key = get-content $nuget_key

  # Find all the packages and display them for confirmation
  $packages = dir $packages_dir -Filter "*.nupkg"
  write-host "Packages to upload:"
  $packages | ForEach-Object { write-host $_.Name }

  # Ensure we haven't run this by accident.
  $result = New-Prompt "Upload Packages" "Do you want to upload the NuGet packages to the NuGet server?" @(
    @("&No", "Does not upload the packages."),
    @("&Yes", "Uploads the packages.")
  )

  # Cancelled
  if ($result -eq 0) {
    "Upload aborted"
  }
  # upload
  elseif ($result -eq 1) {
    $packages | foreach {
      $package = $_.FullName
      write-host "Uploading $package"
      Invoke-Dotnet nuget push $package --api-key $key --source "https://www.nuget.org/api/v2/package"
      write-host
    }
  }
}

target install-dotnet-core {
  # Find the SDK if there's one already.
  findSdk
  # Version check as $IsWindows, $IsLinux etc are not defined in PS 5, only PS Core.
  $win = (($PSVersionTable.PSVersion.Major -le 5) -or $IsWindows)
  $json = ConvertFrom-Json (Get-Content "$path/global.json" -Raw)
  $required_version = $json.sdk.version
  # If there's a version mismatch with what's defined in global.json then a
  # call to dotnet --version will generate an error.
  try { dotnet --version 2>&1>$null } catch { $install_sdk = $true }

  if ($global:LASTEXITCODE) {
    $install_sdk = $true;
    $global:LASTEXITCODE = 0;
  }

  if ($install_sdk) {
    $installer = $null;
    if ($win) {
      $installer = "$build_dir/dotnet-installer.ps1"
      (New-Object System.Net.WebClient).DownloadFile("https://dot.net/v1/dotnet-install.ps1", $installer);
    }
    else {
      $installer = "$build_dir/dotnet-installer"
      write-host Downloading installer to $installer
      curl https://dot.net/v1/dotnet-install.sh -L --output $installer
      chmod +x $installer
    }

    $dotnet_path = "$path/.dotnetsdk"

    # If running in azure pipelines, use that as the dotnet install path.
    if ($env:AGENT_TOOLSDIRECTORY) {
      $dotnet_path = Join-Path $env:AGENT_TOOLSDIRECTORY dotnet
    }

    Write-Host Installing $json.sdk.version to $dotnet_path
    . $installer -i $dotnet_path -v $json.sdk.version

    # Collect installed SDKs.
    $sdks = & "$dotnet_path/dotnet" --list-sdks | ForEach-Object {
      $_.Split(" ")[0]
    }

    # Install any other SDKs required. Only bother installing if not installed already.
    $json.others | Foreach-Object {
      if (!($sdks -contains $_)) {
        Write-Host Installing $_
        . $installer -i $dotnet_path -v $_
      }
    }
    # Set process path again
    findSdk
  }
}

function verify_assembly($path) {
  $asm = [System.Reflection.Assembly]::LoadFile($path);
  $asmName = $asm.GetName().ToString();
  $search = "PublicKeyToken="
  $token = $asmName.Substring($asmName.IndexOf($search) + $search.Length)
  return $token -eq "7de548da2fbae0f0";
}

function findSdk() {
  $dotnet_path = Join-Path $env:AGENT_TOOLSDIRECTORY dotnet

  if (Test-Path $dotnet_path) {
    Write-Host "Using .NET SDK from $dotnet_path"
    $env:DOTNET_INSTALL_DIR = $dotnet_path

    if (($PSVersionTable.PSVersion.Major -le 5) -or $IsWindows) {
      $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
    }
    else {
      # Linux uses colon not semicolon, so can't use string interpolation
      $env:PATH = $env:DOTNET_INSTALL_DIR + ":" + $env:PATH
    }
  }
}

Start-Build $targets
