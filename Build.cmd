@echo off
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe buildscripts\build.proj /p:BuildSilverlight=false %*