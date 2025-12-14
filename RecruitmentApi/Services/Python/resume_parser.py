import re
import json
import argparse
from docx import Document
from PyPDF2 import PdfReader


# -------------------------
# TEXT EXTRACTION
# -------------------------
def extract_text_from_file(file_path):
    text = ""

    if file_path.lower().endswith(".docx"):
        doc = Document(file_path)
        text = "\n".join([p.text for p in doc.paragraphs])

    elif file_path.lower().endswith(".pdf"):
        reader = PdfReader(file_path)
        for page in reader.pages:
            extracted = page.extract_text() or ""
            text += "\n" + extracted

    else:
        raise ValueError("Unsupported file type. Only .docx or .pdf allowed")

    # Clean & normalize
    text = re.sub(r"\s+", " ", text).strip()
    return text


# -------------------------
# EMAIL
# -------------------------
def extract_email(text):
    match = re.search(r"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}", text)
    return match.group(0) if match else ""


# -------------------------
# PHONE
# -------------------------
def extract_phone(text):
    matches = re.findall(r"(\+91[-\s]?\d{10}|\b\d{10}\b)", text)

    for m in matches:
        digits = re.sub(r"\D", "", m)
        if len(digits) >= 10:
            return digits[-10:]

    return ""


# -------------------------
# NAME (Improved)
# -------------------------
def extract_name(text):
    # Split into lines & remove empty lines
    lines = [line.strip() for line in text.split("\n") if line.strip()]

    if not lines:
        return ""

    # Avoid invalid names like "CURRICULUM VITAE", "RESUME", etc.
    ignore_words = ["resume", "curriculum", "vitae", "cv", "profile"]

    for line in lines[:5]:  # Only check top area
        clean = line.lower()
        if not any(w in clean for w in ignore_words) and len(line.split()) <= 5:
            return line.strip()

    return lines[0]  # fallback


# -------------------------
# SKILLS
# -------------------------
def normalize_skill(s):
    """Normalize skill (remove dots, spaces, case-insensitive). Example: 'C++', 'C plus plus' ? 'cplusplus'"""
    s = s.lower()
    s = s.replace(".", "").replace(" ", "").replace("+", "plus")
    return s


def extract_skills(text, known_skills):
    found = []
    text_norm = normalize_skill(text)

    for skill in known_skills:
        skill_norm = normalize_skill(skill)

        # Exact skill match within normalized text
        if skill_norm in text_norm:
            found.append(skill)

    return list(dict.fromkeys(found))  # Remove duplicates while keeping order


# -------------------------
# MAIN PROCESSING
# -------------------------
def process_resume(file_path, skills_list):
    text = extract_text_from_file(file_path)

    return {
        "name": extract_name(text),
        "email": extract_email(text),
        "phone": extract_phone(text),
        "skills": extract_skills(text, skills_list)
    }


# -------------------------
# ENTRY POINT
# -------------------------
if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Resume Parser")
    parser.add_argument("file_path", help="Path to the resume (.pdf/.docx)")
    parser.add_argument("--skills", required=True, help="Comma-separated list of known skills")

    args = parser.parse_args()

    skills_list = [s.strip() for s in args.skills.split(",") if s.strip()]

    result = process_resume(args.file_path, skills_list)
    print(json.dumps(result, indent=2))