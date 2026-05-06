import asyncio
import pandas as pd
from typing import List, Dict, Any
from pathlib import Path
from core.config import Settings, BASE_DIR

# Lock to prevent race conditions during concurrent read/writes
_db_lock = asyncio.Lock()

COLUMN_MAPPING = {
    "FullName": "full_name",
    "ID": "id",
    "Age": "age",
    "Gender": "gender",
    "Diagnosis": "diagnosis",
    "SurgicalStatus": "surgical_status",
    "ClinicalSummary": "clinical_summary"
}

REVERSE_MAPPING = {v: k for k, v in COLUMN_MAPPING.items()}

async def read_patients(settings: Settings) -> List[Dict[str, Any]]:
    """Asynchronously reads patients from the CSV file."""
    csv_path = BASE_DIR / settings.csv_file_path
    
    async with _db_lock:
        def _read() -> List[Dict[str, Any]]:
            if not csv_path.exists():
                return []
            df = pd.read_csv(csv_path)
            # Replace NaNs with empty strings or proper defaults if any missing values
            df = df.fillna("")
            df = df.rename(columns=COLUMN_MAPPING)
            # Ensure 'id' is treated as a string to match the model
            df["id"] = df["id"].astype(str)
            return df.to_dict(orient="records")
        
        # Offload file I/O to a separate thread
        return await asyncio.to_thread(_read)

async def write_patients(settings: Settings, patients_data: List[Dict[str, Any]]) -> None:
    """Asynchronously writes a list of patient dictionaries back to the CSV."""
    csv_path = BASE_DIR / settings.csv_file_path
    
    async with _db_lock:
        def _write() -> None:
            df = pd.DataFrame(patients_data)
            # Ensure columns are in the correct order and names match CSV
            if not df.empty:
                df = df.rename(columns=REVERSE_MAPPING)
                cols = ["FullName", "ID", "Age", "Gender", "Diagnosis", "SurgicalStatus", "ClinicalSummary"]
                df = df[cols]
            # Ensure directory exists before writing
            csv_path.parent.mkdir(parents=True, exist_ok=True)
            df.to_csv(csv_path, index=False)
            
        # Offload file I/O to a separate thread
        await asyncio.to_thread(_write)
