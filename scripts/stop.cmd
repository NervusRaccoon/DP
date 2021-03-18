taskkill /f /IM rankCalculator.exe
taskkill /f /IM valuator.exe

cd "../nginx" & nginx.exe -s quit
nginx -s stop