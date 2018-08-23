param(
  [string]$version = '8.0.0-dev',
  [string]$configuration = 'Release',
  [string]$path = $PSScriptRoot,
  [string]$keyfile = ""
)

$ErrorActionPreference = "Stop"

# Boostrap posh-build
$build_dir = Join-Path $path ".build"
if (! (Test-Path (Join-Path $build_dir "Posh-Build.ps1"))) { Write-Host "Installing posh-build..."; New-Item -Type Directory $build_dir -ErrorAction Ignore | Out-Null; Save-Script "Posh-Build" -Path $build_dir }
. (Join-Path $build_dir "Posh-Build.ps1")

# Set these variables as desired
$packages_dir = Join-Path $build_dir "packages"
$output_dir = Join-Path $build_dir $configuration
$solution_file = Join-Path $path "FluentValidation.sln"
$nuget_key = "$env:USERPROFILE\Dropbox\nuget-access-key.txt"

if (test-path "$env:USERPROFILE\Dropbox\FluentValidation-Release.snk") {
  # Use Jeremy's local copy of the key
  $keyfile = "$env:USERPROFILE\Dropbox\FluentValidation-Release.snk"
}
elseif (Test-Path "$path\src\FluentValidation-Release.snk") {
  # For CI builds appveyor will decrypt the key and place it in src\
  $keyfile = "$path\src\FluentValidation-Release.snk"
}

target default -depends find-sdk, compile, test, deploy
target install -depends install-dotnet-core, decrypt-private-key

target compile {
  Invoke-Dotnet build $solution_file -c $configuration --no-incremental `
    /p:Version=$version /p:AssemblyOriginatorKeyFile=$keyfile
}

target test {
  $test_projects = @(
    "$path\src\FluentValidation.Tests\FluentValidation.Tests.csproj",
    "$path\src\FluentValidation.Tests.Mvc5\FluentValidation.Tests.Mvc5.csproj",
    "$path\src\FluentValidation.Tests.AspNetCore\FluentValidation.Tests.AspNetCore.csproj",
    "$path\src\FluentValidation.Tests.WebApi\FluentValidation.Tests.WebApi.csproj"
  )

  Invoke-Tests $test_projects -c $configuration --no-build
}

target deploy {
  Remove-Item $packages_dir -Force -Recurse -ErrorAction Ignore 2> $null
  Remove-Item $output_dir -Force -Recurse -ErrorAction Ignore 2> $null
  
  Invoke-Dotnet pack $solution_file -c $configuration /p:PackageOutputPath=$packages_dir /p:AssemblyOriginatorKeyFile=$keyfile /p:Version=$version

  # Copy to output dir
  Copy-Item "$path\src\FluentValidation\bin\$configuration" -Destination "$output_dir\FluentValidation" -Recurse
  Copy-Item "$path\src\FluentValidation.Mvc5\bin\$configuration"  -filter FluentValidation.Mvc.* -Destination "$output_dir\FluentValidation.Mvc5-Legacy" -Recurse
  Copy-Item "$path\src\FluentValidation.WebApi\bin\$configuration"  -filter FluentValidation.WebApi.* -Destination "$output_dir\FluentValidation.WebApi-Legacy" -Recurse
  Copy-Item "$path\src\FluentValidation.AspNetCore\bin\$configuration"  -filter FluentValidation.AspNetCore.* -Destination "$output_dir\FluentValidation.AspNetCore" -Recurse
  Copy-Item "$path\src\FluentValidation.ValidatorAttribute\bin\$configuration" -Destination "$output_dir\FluentValidation.ValidatorAttribute" -Recurse
}

target verify-package {
  $sn = "${env:ProgramFiles(x86)}\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\sn.exe"

  if (! (Test-Path $sn)) {
    throw "Could not find sn.exe to verify the dlls"
  }

  if (-not (test-path "$nuget_key")) {
    throw "Could not find the NuGet access key."
  }
  elseif((& $sn -q -v "$output_dir\FluentValidation\net45\FluentValidation.dll") -ne $null) {
    throw "The assemblies have not been signed with a private key."
  }
  else {
    write-host Package verified
  }
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

target decrypt-private-key {
  if (Test-Path ENV:kek) {
    iex ((New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/appveyor/secure-file/master/install.ps1'))
    dotnet "appveyor-tools/secure-file.dll" -decrypt src/FluentValidation-Release.snk.enc -secret $ENV:kek
  }
}

target install-dotnet-core {
  # Ensures that .net core is up to date.
  # first get the required version from global.json
  $json = ConvertFrom-Json (Get-Content "$path/global.json" -Raw)
  $required_version = $json.sdk.version

  # Running dotnet --version stupidly fails if the required SDK version is higher 
  # than the currently installed version. So move global.json out the way 
  # and then put it back again 
  Rename-Item "$path/global.json" "$path/global.json.bak"
  $current_version = (dotnet --version)
  Rename-Item "$path/global.json.bak" "$path/global.json"
  Write-Host "Required .NET version: $required_version Installed: $current_version"

  if ($current_version -lt $required_version) {
    # Current installed version is too low.
    # Install new version as a local only dependency. 

    if (($PSVersionTable.PSVersion.Major -le 5) -or $IsWindows) {
      $urlCurrent = "https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$required_version/dotnet-sdk-$required_version-win-x64.zip"
      Write-Host "Installing .NET Core $required_version from $urlCurrent"
      $env:DOTNET_INSTALL_DIR = "$path/.dotnetsdk"
      New-Item -Type Directory $env:DOTNET_INSTALL_DIR -Force | Out-Null
      (New-Object System.Net.WebClient).DownloadFile($urlCurrent, "dotnet.zip")
      Write-Host "Unzipping to $env:DOTNET_INSTALL_DIR"
      Add-Type -AssemblyName System.IO.Compression.FileSystem; [System.IO.Compression.ZipFile]::ExtractToDirectory("dotnet.zip", $env:DOTNET_INSTALL_DIR)
    }
    elseif ($IsLinux) {
      $urlCurrent = "https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$required_version/dotnet-sdk-$required_version-linux-x64.tar.gz"
      Write-Host "Installing .NET Core $required_version from $urlCurrent"
      $env:DOTNET_INSTALL_DIR = "$path/.dotnetsdk"
      New-Item -Type Directory $env:DOTNET_INSTALL_DIR -Force | Out-Null
      (New-Object System.Net.WebClient).DownloadFile($urlCurrent, "dotnet.tar.gz")
      Write-Host "Unzipping to $env:DOTNET_INSTALL_DIR"
      tar zxvf "dotnet.tar.gz" -C $env:DOTNET_INSTALL_DIR # Use tar directly instead of System.IO.Compression
    }
  }
}

target find-sdk {
  if (Test-Path "$path/.dotnetsdk") {
    Write-Host "Using .NET SDK from $path/.dotnetsdk"
    $env:DOTNET_INSTALL_DIR = "$path/.dotnetsdk"

    if (($PSVersionTable.PSVersion.Major -le 5) -or $IsWindows) {
      $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
    }
    elseif ($IsLinux) {
      $env:PATH = "$env:DOTNET_INSTALL_DIR:$env:PATH" # Linux uses colon not semicolon.
    }
  }
}

Start-Build $args