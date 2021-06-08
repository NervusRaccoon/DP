dotnet build
start "1" dotnet run 8000 localhost 8001 true --no-build
start "2" dotnet run 8001 localhost 8002 --no-build
start "3" dotnet run 8002 localhost 8000 --no-build
