@echo off
if "%1"=="PublishPackages" goto publish

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .build\build.proj /p:BuildSilverlight=false %*
goto end

:publish
powershell.exe -noprofile .build\publish-nuget-packages.ps1

goto end

:end