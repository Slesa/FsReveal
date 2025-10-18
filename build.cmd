@echo off
cls

dotnet paket restore
if errorlevel 1 (
  exit /b %errorlevel%
)

dotnet fake run build.fsx %*
