@echo off
if "%1"=="PublishPackages" goto publish


if exist %WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe set MSBUILD=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
if exist "%ProgramFiles%\MSBuild\14.0\bin\msbuild.exe" set MSBUILD=%ProgramFiles%\MSBuild\14.0\bin\msbuild.exe
if exist "%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild.exe" set MSBUILD=%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild.exe


"%MSBUILD%" .build\build.proj /p:BuildSilverlight=false /p:vs2015=true %*
goto end

:publish
powershell.exe -noprofile .build\publish-nuget-packages.ps1

goto end

:end