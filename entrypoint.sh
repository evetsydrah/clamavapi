# #!/bin/bash

# # Start ClamAV daemon
# echo "Starting ClamAV daemon..."
# freshclam
# clamd &

# # Wait for ClamAV daemon to start
# sleep 10

# # Check for the benchmark argument
# if [[ "$1" == "--benchmark" ]]; then
#     # Change to the app directory where ClamAVApi.csproj is located
#     cd /app
#     # Run the benchmarks
#     echo "Running benchmarks..."
#     dotnet run -c Release -- --benchmark
# else
#     # Start the .NET application
#     echo "Starting .NET application..."
#     dotnet /app/ClamAVApi.dll
# fi
