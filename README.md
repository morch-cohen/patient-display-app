# Project M - Patient Display

## Overview
Project M Patient Display is a high-performance, always-on-top overlay widget designed for clinicians. It allows medical staff to monitor and update patient surgical status (Cleared/Pending) seamlessly while working in other full-screen applications. 

The system consists of a **FastAPI backend** managing persistent data and a **WPF frontend** providing a premium, glassmorphism-inspired user interface.

---

## 🚀 Getting Started

### One-Click Launch (Recommended)
You do **not** need to download the full repository to run the app. You can simply download the `run` folder and use the standalone executables:
1. Open the `run` folder.
2. Double-click `Launch_App.bat`.
3. This will automatically start the background server and launch the Patient Display widget.

### Running from Source
#### Backend (Python)
1. Navigate to the `backend` folder.
2. Create and activate a virtual environment.
3. Install dependencies: `pip install fastapi uvicorn pandas pydantic-settings`.
4. Run: `python main.py`

#### Frontend (.NET)
1. Navigate to the `frontend` folder.
2. Ensure you have .NET 10 SDK installed.
3. Run: `dotnet run`

---

## 🛠 Feature Guide

### 1. The Widget (Logo)
- **Always-on-top**: The circular logo stays visible over other windows.
- **Draggable**: Click and drag the logo anywhere on your screen. It will automatically snap to the edges of your work area.
- **Toggle View**: Click the logo to open or hide the Patient Dashboard.

### 2. Patient Dashboard
- **Search Bar**: Type any name or ID. The search is strictly **alphanumeric** and filtered in real-time with a 350ms debounce.
- **Scrollable List**: View all patients and their current surgical status at a glance.
- **Connection Status**: Clear feedback ("Connecting to server...", "No patients found", or Error messages) ensures you always know the network state.

### 3. Patient Details
- **Detail Window**: Click any patient card to open a dedicated detail view.
- **Update Status**: Toggle between **Cleared** and **Pending**. Changes are saved instantly to the backend CSV.
- **Smart Window Management**: Re-clicking a patient already open will simply bring their window to the front.

---

## 🛑 Shutdown Instructions
Since the application runs processes in the background, follow these steps to fully close the app:

### 1. Normal Shutdown
- **Frontend**: Click the **'X'** button on the Patient Dashboard or close the Widget window.
- **Backend**: Locate the **"Patient Backend"** console window (it may be minimized) and press `Ctrl+C` or close the window.

### 2. Manual Cleanup (If windows are hidden)
If the processes are still running in the background, you can kill them via PowerShell:
- **Stop Backend**: `Stop-Process -Name "patient_backend" -Force`
- **Stop Frontend**: `Stop-Process -Name "PatientDisplay" -Force`

Alternatively, use **Task Manager** to end `patient_backend.exe` and `PatientDisplay.exe` under the Processes tab.

---

## 📁 Project Structure

```text
patient-display-app/
├── Launch_App.bat           # Unified "one-click" launcher
├── PRD.md                   # Product Requirements Document
├── README.md                # Project documentation
├── backend/
│   ├── main.py              # FastAPI Entry point
│   ├── api/                 # API Route definitions
│   ├── core/                # App configuration (settings.py)
│   ├── data/                # CSV storage (patients.csv)
│   ├── models/              # Pydantic data models
│   ├── services/            # Database handling logic
│   └── dist/                # Packaged backend EXE (patient_backend.exe)
└── frontend/
    ├── ViewModels/          # MVVM Business Logic
    ├── Views/               # WPF XAML UI Definitions
    ├── Models/              # Frontend data models
    ├── Services/            # API Client services
    ├── Converters/          # XAML Value Converters
    └── bin/Release/net10.0-windows/win-x64/publish/ # Packaged frontend EXE (PatientDisplay.exe)
```

---

## 🔧 Technical Notes
- **Language/Stack**: Python (FastAPI) + C# (.NET 10 WPF).
- **Persistence**: Local CSV-based storage with async file I/O.
- **Resilience**: 5-try automatic connection retry pattern implemented in the frontend.
- **Validation**: Strict alphanumeric input restriction on the search bar.
- **Stability**: Refined window dragging logic using native OS calls and thread-safe collection management.
