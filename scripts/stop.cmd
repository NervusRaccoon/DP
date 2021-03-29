taskkill /f /IM rankCalculator.exe
taskkill /f /IM valuator.exe
taskkill /f /IM eventsLogger.exe

cd "../nginx" & nginx.exe -s quit
nginx -s stop