cd ..\Valuator\
dotnet build
cd ..\RankCalculator\
dotnet build
cd ..\EventsLogger\
dotnet build

start "localhost:5001" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001"
start "localhost:5002" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5002"

start "RankCalculator1" /d ..\RankCalculator\ dotnet run --no-build
start "RankCalculator2" /d ..\RankCalculator\ dotnet run --no-build

start "EventsLogger1" /d ..\EventsLogger\ dotnet run --no-build
start "EventsLogger2" /d ..\EventsLogger\ dotnet run --no-build

start "nginx" /d ..\nginx\ nginx.exe
