#!/bin/bash
dotnet tool restore
exit_code=$?
if [ $exit_code -ne 0 ]; then
  exit $exit_code
fi

#dotnet paket restore --target-framework "net8.0"
#exit_code=$?
#if [ $exit_code -ne 0 ]; then
#  exit $exit_code
#fi

dotnet fake run build.fsx
dotnet build
