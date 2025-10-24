@echo off
cls

rem dotnet paket restore
rem if errorlevel 1 (
rem   exit /b %errorlevel%
rem )

dotnet fake run build.fsx %*
