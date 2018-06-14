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

  foreach($target_name in $target_names) {
    Execute-Target $target_name
  }

  $timer.Stop()
  $ts = $timer.Elapsed
  $elapsed = [String]::Format("{0:00}:{1:00}.{2:00}",
  $ts.TotalMinutes, $ts.Seconds,
  $ts.Milliseconds / 10);

  write-host
  write-host Build complete  in $elapsed
}

function Execute-Target([string]$name) {
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
    foreach ($to_execute in $targets_to_execute) {
      if ($to_execute.Action) {
        Write-Host »»»»»» $to_execute.Name -ForegroundColor Green
        try {
          & $to_execute.Action;

          if($LASTEXITCODE) {
            throw;
          }
        }
        catch {
          write-host »»»»»» $to_execute.Name failed -ForegroundColor Red
          if ($_.Exception.Message -and $_.Exception.Message -ne 'ScriptHalted') {
            write-host $_.Exception.Message -ForegroundColor Red
          }
          break;
        }
      }
    }
  }
  catch {
    write-host "Target failed " + $_.Exception.Message
    exit(1);
    break;
  }
}

function Invoke-Tests($test_projects, $configuration = 'debug') {
  $has_failures = $false

  $test_projects | % {
    dotnet test $_ -c $configuration -nologo $args
    if ($LASTEXITCODE) { $has_failures = $true }
  }

  if ($has_failures) { throw "Tests failed" }
}

function Invoke-Dotnet {
  write-host $args
  dotnet $args
  if ($LASTEXITCODE) { throw "Dotnet failed" }
}

function New-Prompt($title, $details, $prompt_options, $default_choice = 0) {
  $options = @()

  foreach($key in $prompt_options.Keys) {
    $option = [System.Management.Automation.Host.ChoiceDescription]::new($key, $prompt_options[$key]);
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