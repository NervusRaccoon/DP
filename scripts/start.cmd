start "localhost:5001" /d ..\Valuator\ dotnet run --urls "http://localhost:5001"
start "localhost:5002" /d ..\Valuator\ dotnet run --urls "http://localhost:5002"

start /d ..\RankCalculator\ dotnet run
start /d ..\RankCalculator\ dotnet run

start "nginx" /d ..\nginx\ nginx.exe