@echo off
setlocal

echo ==========================================
echo    PROJECT M - PATIENT DISPLAY LAUNCHER
echo ==========================================
echo.

echo [1/3] Starting Backend Server...
:: Start backend in a minimized window or background
start /min "Patient Backend" "backend\dist\patient_backend.exe"

echo [2/3] Waiting for server to initialize (5s)...
timeout /t 5 /nobreak > nul

echo [3/3] Launching WPF Widget...
start "" "frontend\bin\Release\net10.0-windows\win-x64\publish\PatientDisplay.exe"

echo.
echo Application launched successfully. 
echo Keep this window open or close it as needed.
echo.
pause
