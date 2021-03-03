start "localhost:5001" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001"
start "localhost:5002" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5002"

start "nginx" /d ..\nginx\ nginx.exe