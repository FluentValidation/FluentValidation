param(
    [string]$path # Comes from $(BaseIntermediateOutputPath) msbuild variable
)
$json_file = Join-Path $path "project.assets.json"

$dependences_to_fix = @('FluentValidation', 'FluentValidation.DependencyInjectionExtensions', 'FluentValidation.ValidatorAttribute')
$json = Get-Content $json_file | Out-String | ConvertFrom-Json

$json.libraries.PSObject.Properties | % { 
  $bits = $_.Name.Split("/")
  $package_name = $bits[0]
  $package_version = $bits[1]

  if ($dependences_to_fix -contains $package_name) {
    Write-Host "Rewriting project.assets.json - found $package_name"
    # Found one - change the type.
    $_.Value.type = "package"
    
    # Work out the next major version for the lock 
    $next_major = $package_version.Split(".")[0]
    $next_major = ([int]$next_major) + 1

    # Build the json to inject.
    $json_to_add = "{
      `"target`": `"Package`",
      `"version`": `"[$package_version,$next_major.0)`"
    }"

    # Add to each framework
    $json.project.frameworks.PSObject.Properties | % { 
      $framework = $_.Name
      $json.project.frameworks.${framework}.dependencies | Add-Member -Name $package_name -value (Convertfrom-Json $json_to_add) -MemberType NoteProperty
    }
  }
}

$raw = ConvertTo-Json $json -Depth 32
$raw > $json_file