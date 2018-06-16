@echo off
SET version=7.6.103
powershell -noprofile -ExecutionPolicy Unrestricted -File build.ps1 -Version %version% %*