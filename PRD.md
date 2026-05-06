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
2. **Dashboard:** Clicking the widget toggles the Patient Dashboard window, showing a scrollable list of patients.
3. **Search:** The clinician types in a debounced search bar to filter patients instantly without lagging the UI.
4. **Detail & Action:** Clinician clicks a patient, viewing their detailed information.
5. **Update:** Clinician toggles "Surgical Status" (Cleared/Pending) and saves, triggering a PATCH request to update the backend instantly.
6. **Minimize:** Clinician clicks the widget again to hide the dashboard, returning focus to their main application.

## 4. Technical Architecture
- **Backend:** FastAPI (Python) serving a REST API, backed by a CSV data layer (`patients.csv`).
- **Frontend:** WPF (.NET) utilizing a strict MVVM pattern, maintaining complete separation between UI and business logic.

## 5. Non-Functional Requirements
- **Performance:** Asynchronous file operations and API calls to ensure the UI thread never hangs.
- **Visuals:** Strict adherence to Project M branding (specific hex codes, corner radii, shadows), smooth micro-animations.
- **Usability:** The widget must be draggable but restricted by screen bounds so it cannot be lost off-screen.

## 6. API Specifications

The Project M Patient Display Backend is a RESTful API built with FastAPI.

### Base Configuration
- **Base URL:** `http://localhost:8000` (configurable via `.env`)
- **Documentation:** Interactive Swagger UI available at `/docs`

### Endpoints

#### `GET /patients`
Retrieves a paginated and searchable list of patients.
- **Query Parameters:**
  - `search` (string, optional): Filters patients by name or ID.
  - `page` (integer, default: 1): The page number to retrieve (>= 1).
  - `size` (integer, default: 10): Items per page (1 to 100).
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
  - Standard Patient object (see Data Models).
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
