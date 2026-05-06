from fastapi import FastAPI
from api import patients

app = FastAPI(
    title="Project M - Patient Display API",
    description="FastAPI backend for the Patient Display widget",
    version="1.0.0"
)

# Include the patients router
app.include_router(patients.router)

@app.get("/")
async def root():
    return {"message": "Welcome to the Patient Display API. Visit /docs for the API schema."}

import uvicorn
from core.config import get_settings

if __name__ == "__main__":
    settings = get_settings()
    uvicorn.run("main:app", host="0.0.0.0", port=settings.server_port, reload=True)