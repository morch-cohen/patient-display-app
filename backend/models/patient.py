from pydantic import BaseModel, Field
from enum import Enum
from typing import List

class SurgicalStatusEnum(str, Enum):
    CLEARED = "Cleared"
    PENDING = "Pending"

class GenderEnum(str, Enum):
    MALE = "Male"
    FEMALE = "Female"

class Patient(BaseModel):
    id: str
    full_name: str
    age: int
    gender: GenderEnum
    diagnosis: str
    surgical_status: SurgicalStatusEnum = Field(..., description="Status of the patient, e.g., 'Cleared' or 'Pending'")
    clinical_summary: str

class PatientUpdate(BaseModel):
    surgical_status: SurgicalStatusEnum = Field(..., description="New surgical status to update for the patient")

class PaginatedPatients(BaseModel):
    items: List[Patient]
    total_items: int
    total_pages: int
    current_page: int
