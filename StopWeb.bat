@echo off

REM Navigate to project directory
cd C:\Users\matth\JobPractice\StrudelShopBackend

REM Stop Docker services
echo Stopping Docker services...
docker-compose down

REM Zatvorenie Docker Desktop
echo Zastavuje sa Docker Desktop...
taskkill /F /IM "Docker Desktop.exe" >nul 2>&1

echo Aplikacia bola uspesne zastavena.