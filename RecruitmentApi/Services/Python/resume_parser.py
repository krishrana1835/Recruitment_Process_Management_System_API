import re
import json
import sys
from docx import Document
from PyPDF2 import PdfReader
import argparse

def extract_text_from_file(file_path):
    if file_path.lower().endswith(".docx"):
        doc = Document(file_path)
        return "\n".join([para.text for para in doc.paragraphs])
    elif file_path.lower().endswith(".pdf"):
        reader = PdfReader(file_path)
        return "\n".join([page.extract_text() or "" for page in reader.pages])
    else:
        raise ValueError("Unsupported file type.")

def extract_email(text):
    match = re.search(r"[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+", text)
    return match.group(0) if match else ""

def extract_phone(text):
    matches = re.findall(r"(\+91[-\s]?\d{10}|\b\d{10}\b)", text)
    for match in matches:
        digits = re.sub(r"\D", "", match)
        if len(digits) >= 10:
            return digits[-10:]
    return ""

def extract_name(text):
    lines = [line.strip() for line in text.splitlines() if line.strip()]
    return lines[0] if lines else ""

def extract_skills(text, known_skills):
    found_skills = []
    text_lower = text.lower()
    for skill in known_skills:
        if re.search(rf"\b{re.escape(skill.lower())}\b", text_lower):
            found_skills.append(skill)
    return found_skills

def process_resume(file_path, skills_list):
    text = extract_text_from_file(file_path)
    return {
        "name": extract_name(text),
        "email": extract_email(text),
        "phone": extract_phone(text),
        "skills": extract_skills(text, skills_list)
    }

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Resume Parser")
    parser.add_argument("file_path", help="Path to the resume file (.pdf or .docx)")
    parser.add_argument("--skills", required=True, help="Comma-separated list of known skills")

    args = parser.parse_args()

    # Split comma-separated skills into a list, strip whitespace
    skills_list = [skill.strip() for skill in args.skills.split(",") if skill.strip()]

    result = process_resume(args.file_path, skills_list)
    print(json.dumps(result, indent=2))
