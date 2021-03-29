start "localhost:5001" /d ..\Valuator\ dotnet run --urls "http://localhost:5001"
start "localhost:5002" /d ..\Valuator\ dotnet run --urls "http://localhost:5002"

start "RankCalculator1" /d ..\RankCalculator\ dotnet run
start "RankCalculator2" /d ..\RankCalculator\ dotnet run

start "EventsLogger1" /d ..\EventsLogger\ dotnet run
start "EventsLogger2" /d ..\EventsLogger\ dotnet run

start "nginx" /d ..\nginx\ nginx.exe