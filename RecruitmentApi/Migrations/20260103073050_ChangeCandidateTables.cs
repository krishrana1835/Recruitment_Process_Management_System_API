using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    public partial class ChangeCandidateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // Candidates
            // =========================
            migrationBuilder.RenameColumn("phone", "Candidates", "Phone");
            migrationBuilder.RenameColumn("password", "Candidates", "Password");
            migrationBuilder.RenameColumn("email", "Candidates", "Email");
            migrationBuilder.RenameColumn("resume_path", "Candidates", "ResumePath");
            migrationBuilder.RenameColumn("full_name", "Candidates", "FullName");
            migrationBuilder.RenameColumn("doc_upload", "Candidates", "DocUpload");
            migrationBuilder.RenameColumn("created_at", "Candidates", "CreatedAt");
            migrationBuilder.RenameColumn("candidate_id", "Candidates", "CandidateId");

            // =========================
            // Candidate_Status_History
            // =========================
            migrationBuilder.RenameColumn("status", "Candidate_Status_History", "Status");
            migrationBuilder.RenameColumn("reason", "Candidate_Status_History", "Reason");
            migrationBuilder.RenameColumn("job_id", "Candidate_Status_History", "JobId");
            migrationBuilder.RenameColumn("changed_by", "Candidate_Status_History", "ChangedBy");
            migrationBuilder.RenameColumn("changed_at", "Candidate_Status_History", "ChangedAt");
            migrationBuilder.RenameColumn("candidate_id", "Candidate_Status_History", "CandidateId");
            migrationBuilder.RenameColumn("candidate_status_id", "Candidate_Status_History", "CandidateStatusId");

            // =========================
            // Candidate_Skills
            // =========================
            migrationBuilder.RenameColumn("years_experience", "Candidate_Skills", "YearsExperience");
            migrationBuilder.RenameColumn("skill_id", "Candidate_Skills", "SkillId");
            migrationBuilder.RenameColumn("candidate_id", "Candidate_Skills", "CandidateId");
            migrationBuilder.RenameColumn("candidate_skill_id", "Candidate_Skills", "CandidateSkillId");

            // =========================
            // Candidate_Reviews
            // =========================
            migrationBuilder.RenameColumn("comments", "Candidate_Reviews", "Comments");
            migrationBuilder.RenameColumn("user_id", "Candidate_Reviews", "UserId");
            migrationBuilder.RenameColumn("reviewed_at", "Candidate_Reviews", "ReviewedAt");
            migrationBuilder.RenameColumn("job_id", "Candidate_Reviews", "JobId");
            migrationBuilder.RenameColumn("candidate_id", "Candidate_Reviews", "CandidateId");
            migrationBuilder.RenameColumn("review_id", "Candidate_Reviews", "ReviewId");

            // =========================
            // Candidate_Documents
            // =========================
            migrationBuilder.RenameColumn("verification_status", "Candidate_Documents", "VerificationStatus");
            migrationBuilder.RenameColumn("uploaded_at", "Candidate_Documents", "UploadedAt");
            migrationBuilder.RenameColumn("file_path", "Candidate_Documents", "FilePath");
            migrationBuilder.RenameColumn("document_type", "Candidate_Documents", "DocumentType");
            migrationBuilder.RenameColumn("candidate_id", "Candidate_Documents", "CandidateId");
            migrationBuilder.RenameColumn("document_id", "Candidate_Documents", "DocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally omitted (rename-only migration)
        }
    }
}
