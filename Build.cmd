@echo off
if "%1"=="PublishPackages" goto publish

dotnet build .build\build.proj %*

goto end

:publish
powershell.exe -noprofile .build\publish-nuget-packages.ps1

goto end

:end