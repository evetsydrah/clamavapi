#!/bin/bash

# Start ClamAV daemon
echo "Starting ClamAV daemon..."
freshclam
clamd &

# Wait for ClamAV daemon to start
sleep 10

# Start .NET application
echo "Starting .NET application..."
dotnet /app/ClamAVApi.dll
