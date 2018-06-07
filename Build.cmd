@echo off
if "%1"=="PublishPackages" goto publish

dotnet msbuild .build\build.proj /p:Version=7.6.100 %*

goto end

:publish
powershell.exe -noprofile .build\publish-nuget-packages.ps1

goto end

:end