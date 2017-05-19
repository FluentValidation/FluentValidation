@echo off
if "%1"=="PublishPackages" goto publish


rem if exist %WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe set MSBUILD=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
rem if exist "%ProgramFiles%\MSBuild\14.0\bin\msbuild.exe" set MSBUILD=%ProgramFiles%\MSBuild\14.0\bin\msbuild.exe
rem if exist "%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild.exe" set MSBUILD=%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild.exe

if exist "%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe" set MSBUILD="%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe" set MSBUILD="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"

if exist "%ProgramFiles%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe" set MSBUILD="%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe" set MSBUILD="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"

if exist "%ProgramFiles%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" set MSBUILD="%ProgramFiles%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" set MSBUILD="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"

echo Using MSBuild from %MSBUILD%

rem /p:NugetVersion=6.4.1
%MSBUILD% .build\build.proj /p:Version=7.0.0 %*
goto end

:publish
powershell.exe -noprofile .build\publish-nuget-packages.ps1

goto end

:end