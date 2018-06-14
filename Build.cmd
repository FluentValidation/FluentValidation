@echo off
SET version=7.6.102
pwsh -noprofile -ExecutionPolicy Unrestricted -File build.ps1 -Version %version% %*