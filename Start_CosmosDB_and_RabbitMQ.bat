start "" "C:\Program Files\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe"
start "" "C:\Program Files\Docker\Docker\Docker Desktop.exe"
timeout /T 20
start "" "http://localhost:15672/"
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
@echo off
pause >nul