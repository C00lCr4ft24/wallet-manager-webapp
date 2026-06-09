@echo off
start "WebApp Server" cmd /k "cd /d WebApp.Server && dotnet run --configuration Debug"
start "WebApp Client" cmd /k "cd /d WebApp.Client && npm run dev"