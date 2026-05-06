@echo off
setlocal

echo ==========================================
echo    PROJECT M - PATIENT DISPLAY LAUNCHER
echo ==========================================
echo.

echo [1/3] Starting Backend Server...
:: Start backend in a minimized window
start /min "Patient Backend" "patient_backend.exe"

echo [2/3] Waiting for server to initialize (5s)...
timeout /t 5 /nobreak > nul

echo [3/3] Launching WPF Widget...
start "" "PatientDisplay.exe"

echo.
echo Application launched successfully. 
echo.
pause
