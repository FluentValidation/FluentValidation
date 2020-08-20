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
  Invoke-Dotnet build $solution_file -c $configuration --no-incremental
}

target test {
  Invoke-Dotnet test $solution_file -c $configuration --no-build --logger trx
}

target deploy {
  Invoke-Dotnet pack $solution_file -c $configuration
}

target verify-package {
  Get-ChildItem $output_dir -Recurse *.dll | ForEach {
    $asm = $_.FullName
    if (! (verify_assembly $asm)) {
      throw "$asm is not signed"
    }
  }
  write-host Package verified
}

target publish -depends verify-package {
  $interactive = $true
  $key = $Env:NUGET_API_KEY

  if ($key) {
    $interactive = $false
  }
  elseif (test-path "$nuget_key") {
    $key = get-content $nuget_key
    $interactive = $true
  }
  else {
    throw "NUGET_API_KEY or local key not set"
  }

  # Find all the packages and display them for confirmation
  $packages = dir $packages_dir -Filter "*.nupkg"
  write-host "Packages to upload:"
  $packages | ForEach-Object { write-host $_.Name }

  if ($interactive) {
    # Ensure we haven't run this by accident.
    $proceed = New-Prompt "Upload Packages" "Do you want to upload the NuGet packages to the NuGet server?" @(
      @("&No", "Does not upload the packages."),
      @("&Yes", "Uploads the packages.")
    )
  }
  else {
    $proceed = 1;
  }

  # Cancelled
  if ($proceed -eq 0) {
    "Upload aborted"
  }
  # upload
  elseif ($proceed -eq 1) {
    $packages | foreach {
      $package = $_.FullName
      Write-Host "Uploading $package"
      Invoke-Dotnet nuget push $package --api-key $key --source "https://www.nuget.org/api/v2/package"
      Write-Host
    }
  }
}

function verify_assembly($path) {
  $asm = [System.Reflection.Assembly]::LoadFile($path);
  $asmName = $asm.GetName().ToString();
  $search = "PublicKeyToken="
  $token = $asmName.Substring($asmName.IndexOf($search) + $search.Length)
  return $token -eq "7de548da2fbae0f0";
}

Start-Build $targets
