from fastapi import APIRouter, HTTPException, Query, Depends
from typing import List, Optional
import math

from models.patient import Patient, PatientUpdate, PaginatedPatients
from services import database_handler
from core.config import get_settings, Settings, settings

router = APIRouter(prefix="/patients", tags=["patients"])

@router.get("/", response_model=PaginatedPatients)
async def get_patients(
    search: Optional[str] = Query(None, description="Search by patient name or ID"),
    page: int = Query(1, ge=1, description="Page number"),
    size: int = Query(
        settings.default_page_size, 
        ge=1, 
        le=settings.max_page_size, 
        description="Items per page"
    ),
    settings: Settings = Depends(get_settings)
):
    """
    Retrieves a paginated list of patients. Supports optional search query.
    """
    patients_data = await database_handler.read_patients(settings)
    
    if search:
        search_lower = search.lower()
        patient_keys = Patient.model_fields.keys()
        patients_data = [
            p for p in patients_data 
            if any(search_lower in str(p.get(key, "")).lower() for key in patient_keys)
        ]
        
    patients = [Patient(**p) for p in patients_data]
        
    total_items = len(patients)
    total_pages = math.ceil(total_items / size) if total_items > 0 else 0
    
    start_idx = (page - 1) * size
    end_idx = start_idx + size
    paginated_items = patients[start_idx:end_idx]
        
    return PaginatedPatients(
        items=paginated_items,
        total_items=total_items,
        total_pages=total_pages,
        current_page=page
    )

@router.get("/{patient_id}", response_model=Patient)
async def get_patient(patient_id: str, settings: Settings = Depends(get_settings)):
    """
    Retrieves detailed information for a specific patient by ID.
    """
    patients_data = await database_handler.read_patients(settings)
    for p_data in patients_data:
        if str(p_data["id"]) == patient_id:
            return Patient(**p_data)
            
    raise HTTPException(status_code=404, detail="Patient not found")

@router.patch("/{patient_id}", response_model=Patient)
async def update_patient(
    patient_id: str, 
    update_data: PatientUpdate,
    settings: Settings = Depends(get_settings)
):
    """
    Updates specific fields for a patient (e.g., surgical_status).
    """
    patients_data = await database_handler.read_patients(settings)
    
    patient_idx = None
    for i, p_data in enumerate(patients_data):
        if str(p_data["id"]) == patient_id:
            patient_idx = i
            break
            
    if patient_idx is None:
        raise HTTPException(status_code=404, detail="Patient not found")
        
    # Apply updates using the enum value
    patients_data[patient_idx]["surgical_status"] = update_data.surgical_status.value
    
    # Persist changes
    try:
        await database_handler.write_patients(settings, patients_data)
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Failed to save patient data: {str(e)}")
    
    return Patient(**patients_data[patient_idx])
