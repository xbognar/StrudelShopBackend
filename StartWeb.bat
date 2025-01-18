@echo off

REM Check if Docker Desktop is running
tasklist /FI "IMAGENAME eq Docker Desktop.exe" 2>NUL | find /I /N "Docker Desktop.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo Docker Desktop is already running.
) else (
    echo Starting Docker Desktop...
    powershell -Command "Start-Process 'C:\Program Files\Docker\Docker\Docker Desktop.exe' -WindowStyle Minimized"
    echo Waiting for Docker Desktop to start...
)

REM Verify Docker is ready and retry
:check_docker_ready
docker info >nul 2>&1
if errorlevel 1 (
    echo Docker is not ready yet. Retrying in 5 seconds...
    timeout /t 5 >nul
    goto check_docker_ready
)

REM Navigate to project directory
cd C:\Users\matth\JobPractice\StrudelShopBackend

REM Start Docker services
echo Starting Docker services...
docker-compose up --build -d

echo Application successfully started.
