import sys
from pydantic_settings import BaseSettings
from functools import lru_cache
from pathlib import Path

# Handle PyInstaller's temp directory vs source directory
if getattr(sys, 'frozen', False):
    BASE_DIR = Path(sys._MEIPASS)
else:
    BASE_DIR = Path(__file__).resolve().parent.parent

class Settings(BaseSettings):
    csv_file_path: str
    default_page_size: int
    max_page_size: int
    server_port: int

    class Config:
        env_file = str(BASE_DIR / ".env")

@lru_cache()
def get_settings() -> Settings:
    return Settings()

# Instantiate globally for route decorators
settings = get_settings()
