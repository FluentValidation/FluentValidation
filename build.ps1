param(
  [string]$configuration = 'Release',
  [string]$path = $PSScriptRoot,
  [string]$keyfile = "",
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
$keyfile = Resolve-Path "~/Dropbox/FluentValidation-Release.snk" -ErrorAction Ignore 
$nuget_key = Resolve-Path "~/Dropbox/nuget-access-key.txt" -ErrorAction Ignore

target default -depends compile, test, deploy
target ci -depends ci-set-version, decrypt-private-key, default

$script:version_suffix = ([xml](get-content src/Directory.Build.props)).Project.PropertyGroup.VersionSuffix

target compile {
  if ($keyfile) {
    Write-Host "Using key file: $keyfile" -ForegroundColor Cyan
  }

  Invoke-Dotnet build $solution_file -c $configuration --no-incremental `
    /p:AssemblyOriginatorKeyFile=$keyfile /p:VersionSuffix=$script:version_suffix
}

target test {
  Invoke-Dotnet test $solution_file -c $configuration --no-build --logger trx 
}

target deploy {
  Remove-Item $packages_dir -Force -Recurse -ErrorAction Ignore 2> $null
  Remove-Item $output_dir -Force -Recurse -ErrorAction Ignore 2> $null
  
  Invoke-Dotnet pack $solution_file -c $configuration /p:PackageOutputPath=$packages_dir /p:AssemblyOriginatorKeyFile=$keyfile /p:VersionSuffix=$script:version_suffix

  # Copy to output dir
  Copy-Item "$path\src\FluentValidation\bin\$configuration" -Destination "$output_dir\FluentValidation" -Recurse
  Copy-Item "$path\src\FluentValidation.Mvc5\bin\$configuration"  -filter FluentValidation.Mvc.* -Destination "$output_dir\FluentValidation.Mvc5-Legacy" -Recurse
  Copy-Item "$path\src\FluentValidation.WebApi\bin\$configuration"  -filter FluentValidation.WebApi.* -Destination "$output_dir\FluentValidation.WebApi-Legacy" -Recurse
  Copy-Item "$path\src\FluentValidation.AspNetCore\bin\$configuration"  -filter FluentValidation.AspNetCore.* -Destination "$output_dir\FluentValidation.AspNetCore" -Recurse
  Copy-Item "$path\src\FluentValidation.ValidatorAttribute\bin\$configuration" -Destination "$output_dir\FluentValidation.ValidatorAttribute" -Recurse
  Copy-Item "$path\src\FluentValidation.DependencyInjectionExtensions\bin\$configuration" -Destination "$output_dir\FluentValidation.DependencyInjectionExtensions" -Recurse
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

target ci-set-version { 
  if ($env:BUILD_BUILDNUMBER) {
    # If there's a build number environment variable provided by CI, use that for the build number suffix.
    $script:version_suffix = "ci-${env:BUILD_BUILDNUMBER}"
  }
}

target decrypt-private-key {
  if ((Test-Path ENV:kek) -and ($ENV:kek.Length -gt 0) -and ($env:SYSTEM_PULLREQUEST.ISFORK -eq $false)) {
    iex ((New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/appveyor/secure-file/master/install.ps1')) | Out-Null
    dotnet "appveyor-tools/secure-file.dll" -decrypt src/FluentValidation-Release.snk.enc -secret $ENV:kek
    if (($LASTEXITCODE -eq 0) -and (Test-Path "$path/src/FluentValidation-Release.snk")) {
      $script:keyfile = "$path/src/FluentValidation-Release.snk";
      Write-Host "Decrypted."
    }
  }
  else {
    Write-Host "No KEK available to decrypt private key."
  }
}

target get-dotnet-version {
  $json = ConvertFrom-Json (Get-Content "$path/global.json" -Raw)
  $required_version = $json.sdk.version
  Write-Host "Required SDK: $required_version"
  # Special syntax to pass a variable back to azure pipelines.
  echo "##vso[task.setvariable variable=dotnetVersion]$required_version"
}

function verify_assembly($path) {
  $asm = [System.Reflection.Assembly]::LoadFile($path);
  $asmName = $asm.GetName().ToString();
  $search = "PublicKeyToken="
  $token = $asmName.Substring($asmName.IndexOf($search) + $search.Length)
  return $token -eq "7de548da2fbae0f0";
}

Start-Build $targets
