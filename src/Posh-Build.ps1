# dotnet build utilities for Powershell by Jeremy Skinner
# For the latest version of this file, go to https://github.com/jeremyskinner/posh-build

$script:Targets = [System.Collections.Generic.Dictionary[string, [BuildTarget]]]::new()
function target {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory = $true)]
    [string]$name,
    [scriptblock]$action = $null,
    [string[]]$depends = @()
  )

  if ($script:Targets.ContainsKey($name)) {
    throw "Target $name already defined.";
  }

  $target = [BuildTarget]::new($action, $depends, $name);
  $script:Targets.Add($name, $target);
}

function Start-Build($params = @()) {
  $timer = [System.Diagnostics.Stopwatch]::new()
  $timer.Start()

  $target_names = @()

  for($i = 0; $i -lt $params.Count; $i++) {
    if ($params[$i] -eq '-t' -or $params[$i] -eq '-targets') {
      if ($params[$i+1]) {
        $target_names += $params[$i+1];
      }
    }
  }

  if ($target_names.Count -eq 0) {
    $target_names += 'default'
  }

  # Verify all target names before running
  foreach($target_name in $target_names) {
    if (-not $script:Targets.ContainsKey($target_names)) {
      throw "Target $target_name not found"
    }
  }

  $exit_code = 0;
  foreach($target_name in $target_names) {
    $target_success = Execute-Target $target_name
    
    if (!$target_success) {
      $exit_code = 1;
    }
  }

  $timer.Stop()
  $ts = $timer.Elapsed
  $elapsed = [String]::Format("{0:00}:{1:00}.{2:00}",
  $ts.TotalMinutes, $ts.Seconds,
  $ts.Milliseconds / 10);

  Write-Host ""

  Write-Host "Build " -NoNewline
  if ($exit_code) {
    write-host "Failed " -NoNewline -ForegroundColor Red
  }
  else {
    write-Host "Completed " -NoNewline -ForegroundColor Green
  }
  write-host "in $elapsed"

  exit($exit_code)
}

function Execute-Target([string]$name) {
  $success = $true;
  $target = $null;

  if (-not $script:Targets.ContainsKey($name)) {
    throw "Target $name not found"
  }

  $target = $script:Targets[$name];
  $targets_to_execute = [System.Collections.Generic.List[BuildTarget]]::new();

  foreach ($target_in_sequence in $target.GetExecutionSequence()) {
    if (!$targets_to_execute.Contains($target_in_sequence)) {
      $targets_to_execute.Add($target_in_sequence);
    }
  }

  try {
    $status_prefix = (1..10 | Foreach-Object { [char]0xBB; }) -Join "" # »»»»»»»»»» 

    foreach ($to_execute in $targets_to_execute) {
      if ($to_execute.Action) {
        Write-Host $status_prefix $to_execute.Name -ForegroundColor Green
        try {
          & $to_execute.Action | Out-Default;

          if($LASTEXITCODE) {
            throw;
          }
        }
        catch {
          $success = $false;
          write-host $status_prefix $to_execute.Name failed -ForegroundColor Red
          if ($_.Exception.Message -and $_.Exception.Message -ne 'ScriptHalted') {
            write-host $_.Exception.Message -ForegroundColor Red
          }
          break;
        }
      }
    }
  }
  catch {
    $success = $false
    write-host "Target failed " + $_.Exception.Message
    exit(1);
    break;
  }
  return $success
}

function Invoke-Tests($test_projects, $configuration = 'debug') {
  $has_failures = $false

  foreach($project in $test_projects) {
    & dotnet test $project -c $configuration -nologo $args
    if ($LASTEXITCODE) { $has_failures = $true }
  }

  if ($has_failures) { throw '' }
}

function Invoke-Xunit($test_projects, $configuration = 'debug') {
  $has_failures = $false

  foreach($project in $test_projects) {
    # CD into the directory with the project
    $dir = (Get-Item $project).Directory
    Push-Location $dir
    & dotnet xunit -c $configuration -nologo $args
    Pop-Location
    if ($LASTEXITCODE) { $has_failures = $true }
  }

  if ($has_failures) { throw '' }
}

function Invoke-Dotnet {
  write-host $args
  dotnet $args
  if ($LASTEXITCODE) { throw "Dotnet failed" }
}

function New-Prompt($title, $details, $prompt_options, $default_choice = 0) {
  $options = @()

  foreach($entry in $prompt_options) {
    $key = $entry[0]
    $text = $entry[1]

    $option = [System.Management.Automation.Host.ChoiceDescription]::new($key, $text);
    $options += $option
  }

  return $host.ui.PromptForChoice($title, $details, $options, $default_choice)
}

class BuildTarget {
  [scriptblock]$Action;
  [string[]]$Dependencies;
  [string]$Name;

  BuildTarget([scriptblock]$action, [string[]] $dependencies, [string]$name) {
    $this.Action = $action;
    $this.Dependencies = $dependencies;
    $this.Name = $name;
  }

  [System.Collections.Generic.List[BuildTarget]] GetExecutionSequence() {
    $execution_sequence = [System.Collections.Generic.List[BuildTarget]]::new()
    $parsed_sequence = [System.Collections.Generic.List[BuildTarget]]::new()

    $this.PopulateExecutionSequence($execution_sequence, $parsed_sequence);

    $execution_sequence.Reverse()
    return $execution_sequence;
  }

  PopulateExecutionSequence([System.Collections.Generic.List[BuildTarget]] $execution_sequence, [System.Collections.Generic.List[BuildTarget]] $parsed_sequence) {
    if ($parsed_sequence.Contains($this)) {
      return;
    }

    $n = $this.Name;

    if ($execution_sequence.Contains($this)) {
      throw "Target ${n} has recursive dependencies"
    }

    $execution_sequence.Add($this);

    $reversed = [System.Collections.Generic.List[string]]::new($this.Dependencies);
    $reversed.Reverse()

    foreach ($dependency_name in $reversed) {
      $dependency = $this.EnsureTargetExists($dependency_name);
      $dependency.PopulateExecutionSequence($execution_sequence, $parsed_sequence);
    }

    $parsed_sequence.Add($this);
  }

  [BuildTarget] EnsureTargetExists([string]$dependency_name) {
    if (! $script:Targets.ContainsKey($dependency_name)) {
      throw "Target $dependency_name could not be found";
    }

    return $script:Targets[$dependency_name];
  }
}