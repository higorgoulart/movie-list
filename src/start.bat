@echo off
setlocal enabledelayedexpansion

echo Building solutions...
call "%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "%CD%\backend\MovieList.sln" -t:rebuild -p:Configuration=Debug
if %ERRORLEVEL% neq 0 (
    echo Build failed. Check errors!
    pause
    exit /b 1
)

echo Starting Docker Compose...
docker compose up -d

set "CONTAINER_FILTER=name=mssqltools"
set MAX_RETRIES=10
set RETRY_INTERVAL=10

echo Waiting for container to complete...
for /l %%i in (1,1,%MAX_RETRIES%) do (
    docker ps --filter %CONTAINER_FILTER% --format "{{.Names}}" | findstr /i /c:"mssqltools" >nul
    if errorlevel 1 (
        echo Container mssqltools finalized!
        goto :container_stopped
    )
    echo Container still running [attempt %%i/%MAX_RETRIES%]
    timeout /t %RETRY_INTERVAL% >nul
)

echo Error: Timeout waiting for container
exit /b 1

:container_stopped
echo Starting IIS applications...
start "WorkerService" C:\PROGRA~1\IISEXP~1\iisexpress.exe /path:"%CD%\backend\MovieList.WorkerService" /port:5000
start "WebAPI" C:\PROGRA~1\IISEXP~1\iisexpress.exe /path:"%CD%\backend\MovieList.WebAPI" /port:5001

echo Starting Keep-Alive script...
start "" /B powershell -ExecutionPolicy Bypass -File "%~dp0ping.ps1"

echo All services started.
pause