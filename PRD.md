# Product Requirements Document (PRD): Patient Display

## 1. Product Overview
Project M Patient Display is an overlay widget designed for clinicians to quickly view and manage patient status without disrupting their main workflow. It acts as a lightweight, always-on-top dashboard that surfaces critical data seamlessly.

## 2. User Personas
**Clinicians (Nurses, Surgeons, Anesthesiologists):**
- Work in a fast-paced environment.
- Need instant access to patient surgical readiness (Cleared vs. Pending).
- Cannot afford to switch away from critical full-screen medical applications.

## 3. Core Workflow
1. **Launch:** The clinician interacts with the Project M widget (an always-on-top, draggable circular logo).
2. **Dashboard:** Clicking the widget toggles the Patient Dashboard window, which shows a paginated list of patients.
3. **Search:** The clinician types in a debounced, alphanumeric-only search bar to filter patients instantly.
4. **Detail & Action:** Clinician clicks a patient, viewing their detailed information (Identity and Diagnosis).
5. **Update:** Clinician toggles "Surgical Status" (Cleared/Pending), which triggers an immediate persistence to the backend.
6. **Minimize:** Clinician clicks the widget again or the close button to hide the dashboard, returning focus to their main application.

## 4. Technical Architecture
- **Backend:** FastAPI (Python 3.14+) serving a REST API, backed by a persistent CSV data layer (`mock_patient_data.csv`). Distributed as a standalone executable.
- **Frontend:** WPF (.NET 10) utilizing a strict MVVM pattern with CommunityToolkit.Mvvm. Distributed as a single-file, self-contained executable.
- **Connectivity:** Robust HTTP client with a 5-try automatic retry mechanism and visual connection status feedback.

## 5. Non-Functional Requirements
- **Performance:** Asynchronous file operations and API calls to ensure the UI thread never hangs. Debounced search (350ms) to prevent server hammering.
- **Visuals:** Strict adherence to Project M branding (Glassmorphism, colors of Project M, rounded corners for elements), smooth micro-animations.
- **Usability:** 
    - The widget is draggable and restricted by screen bounds.
    - Search input is restricted to alphanumeric characters to prevent invalid queries.
    - Clear visual feedback for "Loading", "Connecting", "No Results", and "Connection Failed" states.
- **Stability:** Native `DragMove()` for fluid window movement and LINQ-based collection management to prevent "Collection Modified" crashes.

## 6. API Specifications

The Project M Patient Display Backend is a RESTful API built with FastAPI.

### Base Configuration
- **Base URL:** `http://localhost:8000` (configurable via `.env`)
- **Documentation:** Interactive Swagger UI available at `/docs`

### Endpoints

#### `GET /patients`
Retrieves a paginated and searchable list of patients.
- **Query Parameters:**
  - `search` (string, optional): Filters patients by name or ID (alphanumeric).
  - `page` (integer, default: 1): The page number to retrieve (>= 1).
  - `size` (integer, default: 10): Items per page.
- **Response (PaginatedPatients):**
  - `items` (List[Patient]): List of patient objects.
  - `total_items` (integer): Total count of patients matching the query.
  - `total_pages` (integer): Total number of pages based on size.
  - `current_page` (integer): The requested page number.

#### `GET /patients/{id}`
Retrieves detailed information for a specific patient.
- **Path Parameters:**
  - `id` (string): The unique identifier of the patient.
- **Response (Patient):**
  - Standard Patient object.
- **Errors:**
  - `404 Not Found`: Patient with the specified ID does not exist.

#### `PATCH /patients/{id}`
Updates the surgical status of a patient.
- **Path Parameters:**
  - `id` (string): The unique identifier of the patient.
- **Request Body (PatientUpdate):**
  - `surgical_status` (string): Must be either `"Cleared"` or `"Pending"`.
- **Response (Patient):**
  - The updated patient object.
- **Errors:**
  - `404 Not Found`: Patient with the specified ID does not exist.
  - `500 Internal Server Error`: Disk error or file-locking issue during persistence.

### Data Models

#### Patient
- `id` (string): Unique identifier.
- `full_name` (string): Full name of the patient.
- `age` (integer): Patient's age.
- `gender` (string): `"Male"` or `"Female"`.
- `diagnosis` (string): Clinical diagnosis.
- `surgical_status` (string): `"Cleared"` or `"Pending"`.
- `clinical_summary` (string): Brief medical notes.
