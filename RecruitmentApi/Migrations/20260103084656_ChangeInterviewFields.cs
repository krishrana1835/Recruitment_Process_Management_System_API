using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApi.Migrations
{
    public partial class ChangeInterviewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------- Interviews Columns ----------
            migrationBuilder.RenameColumn("status", "Interviews", "Status");
            migrationBuilder.RenameColumn("score", "Interviews", "Score");
            migrationBuilder.RenameColumn("mode", "Interviews", "Mode");
            migrationBuilder.RenameColumn("start_time", "Interviews", "StartTime");
            migrationBuilder.RenameColumn("scheduled_by", "Interviews", "ScheduledBy");
            migrationBuilder.RenameColumn("round_number", "Interviews", "RoundNumber");
            migrationBuilder.RenameColumn("location_or_link", "Interviews", "LocationOrLink");
            migrationBuilder.RenameColumn("job_id", "Interviews", "JobId");
            migrationBuilder.RenameColumn("interview_type_id", "Interviews", "InterviewTypeId");
            migrationBuilder.RenameColumn("end_time", "Interviews", "EndTime");
            migrationBuilder.RenameColumn("candidate_id", "Interviews", "CandidateId");
            migrationBuilder.RenameColumn("interview_id", "Interviews", "InterviewId");

            // ---------- Create Indexes for Interviews ----------
            migrationBuilder.CreateIndex(
                name: "IX_Interviews_JobId",
                table: "Interviews",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewTypeId",
                table: "Interviews",
                column: "InterviewTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CandidateId",
                table: "Interviews",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ScheduledBy",
                table: "Interviews",
                column: "ScheduledBy");

            // ---------- Interview_Type Columns ----------
            migrationBuilder.RenameColumn("process_descreption", "Interview_Type", "ProcessDescreption");
            migrationBuilder.RenameColumn("interview_round_name", "Interview_Type", "InterviewRoundName");
            migrationBuilder.RenameColumn("interview_type_id", "Interview_Type", "InterviewTypeId");

            // ---------- Interview_Feedback Columns ----------
            migrationBuilder.RenameColumn("comments", "Interview_Feedback", "Comments");
            migrationBuilder.RenameColumn("user_id", "Interview_Feedback", "UserId");
            migrationBuilder.RenameColumn("technical_rating", "Interview_Feedback", "TechnicalRating");
            migrationBuilder.RenameColumn("interview_id", "Interview_Feedback", "InterviewId");
            migrationBuilder.RenameColumn("feedback_at", "Interview_Feedback", "FeedbackAt");
            migrationBuilder.RenameColumn("concept_rating", "Interview_Feedback", "ConceptRating");
            migrationBuilder.RenameColumn("candidate_skill_id", "Interview_Feedback", "CandidateSkillId");
            migrationBuilder.RenameColumn("feedback_id", "Interview_Feedback", "FeedbackId");

            // ---------- Create Indexes for Interview_Feedback ----------
            migrationBuilder.CreateIndex(
                name: "IX_Interview_Feedback_UserId",
                table: "Interview_Feedback",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_Feedback_InterviewId",
                table: "Interview_Feedback",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_Feedback_CandidateSkillId",
                table: "Interview_Feedback",
                column: "CandidateSkillId");

            // ---------- HR_Review Columns ----------
            migrationBuilder.RenameColumn("strengths", "HR_Review", "Strengths");
            migrationBuilder.RenameColumn("user_id", "HR_Review", "UserId");
            migrationBuilder.RenameColumn("training_recommendations", "HR_Review", "TrainingRecommendations");
            migrationBuilder.RenameColumn("teamwork_rating", "HR_Review", "TeamworkRating");
            migrationBuilder.RenameColumn("overall_rating", "HR_Review", "OverallRating");
            migrationBuilder.RenameColumn("leadership_rating", "HR_Review", "LeadershipRating");
            migrationBuilder.RenameColumn("interview_id", "HR_Review", "InterviewId");
            migrationBuilder.RenameColumn("communication_rating", "HR_Review", "CommunicationRating");
            migrationBuilder.RenameColumn("career_path_notes", "HR_Review", "CareerPathNotes");
            migrationBuilder.RenameColumn("areas_for_improvement", "HR_Review", "AreasForImprovement");
            migrationBuilder.RenameColumn("adaptability_rating", "HR_Review", "AdaptabilityRating");
            migrationBuilder.RenameColumn("review_id", "HR_Review", "ReviewId");

            // ---------- Create Indexes for HR_Review ----------
            migrationBuilder.CreateIndex(
                name: "IX_HR_Review_UserId",
                table: "HR_Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_Review_InterviewId",
                table: "HR_Review",
                column: "InterviewId");

            // ---------- Employee_Records Columns ----------
            migrationBuilder.RenameColumn("offer_letter_path", "Employee_Records", "OfferLetterPath");
            migrationBuilder.RenameColumn("joining_date", "Employee_Records", "JoiningDate");
            migrationBuilder.RenameColumn("job_id", "Employee_Records", "JobId");
            migrationBuilder.RenameColumn("candidate_id", "Employee_Records", "CandidateId");
            migrationBuilder.RenameColumn("employee_id", "Employee_Records", "EmployeeId");

            // ---------- Create Index for Employee_Records ----------
            migrationBuilder.CreateIndex(
                name: "IX_Employee_Records_JobId",
                table: "Employee_Records",
                column: "JobId");

            // ---------- EmailRecipients Columns ----------
            migrationBuilder.RenameColumn("email", "EmailRecipients", "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ---------- Reverse Columns ----------
            migrationBuilder.RenameColumn("Status", "Interviews", "status");
            migrationBuilder.RenameColumn("Score", "Interviews", "score");
            migrationBuilder.RenameColumn("Mode", "Interviews", "mode");
            migrationBuilder.RenameColumn("StartTime", "Interviews", "start_time");
            migrationBuilder.RenameColumn("ScheduledBy", "Interviews", "scheduled_by");
            migrationBuilder.RenameColumn("RoundNumber", "Interviews", "round_number");
            migrationBuilder.RenameColumn("LocationOrLink", "Interviews", "location_or_link");
            migrationBuilder.RenameColumn("JobId", "Interviews", "job_id");
            migrationBuilder.RenameColumn("InterviewTypeId", "Interviews", "interview_type_id");
            migrationBuilder.RenameColumn("EndTime", "Interviews", "end_time");
            migrationBuilder.RenameColumn("CandidateId", "Interviews", "candidate_id");
            migrationBuilder.RenameColumn("InterviewId", "Interviews", "interview_id");

            // ---------- Drop Indexes ----------
            migrationBuilder.DropIndex("IX_Interviews_JobId", "Interviews");
            migrationBuilder.DropIndex("IX_Interviews_InterviewTypeId", "Interviews");
            migrationBuilder.DropIndex("IX_Interviews_CandidateId", "Interviews");
            migrationBuilder.DropIndex("IX_Interviews_ScheduledBy", "Interviews");

            // ---------- Interview_Type ----------
            migrationBuilder.RenameColumn("ProcessDescreption", "Interview_Type", "process_descreption");
            migrationBuilder.RenameColumn("InterviewRoundName", "Interview_Type", "interview_round_name");
            migrationBuilder.RenameColumn("InterviewTypeId", "Interview_Type", "interview_type_id");

            // ---------- Interview_Feedback ----------
            migrationBuilder.RenameColumn("Comments", "Interview_Feedback", "comments");
            migrationBuilder.RenameColumn("UserId", "Interview_Feedback", "user_id");
            migrationBuilder.RenameColumn("TechnicalRating", "Interview_Feedback", "technical_rating");
            migrationBuilder.RenameColumn("InterviewId", "Interview_Feedback", "interview_id");
            migrationBuilder.RenameColumn("FeedbackAt", "Interview_Feedback", "feedback_at");
            migrationBuilder.RenameColumn("ConceptRating", "Interview_Feedback", "concept_rating");
            migrationBuilder.RenameColumn("CandidateSkillId", "Interview_Feedback", "candidate_skill_id");
            migrationBuilder.RenameColumn("FeedbackId", "Interview_Feedback", "feedback_id");

            // ---------- Drop Indexes ----------
            migrationBuilder.DropIndex("IX_Interview_Feedback_UserId", "Interview_Feedback");
            migrationBuilder.DropIndex("IX_Interview_Feedback_InterviewId", "Interview_Feedback");
            migrationBuilder.DropIndex("IX_Interview_Feedback_CandidateSkillId", "Interview_Feedback");

            // ---------- HR_Review ----------
            migrationBuilder.RenameColumn("Strengths", "HR_Review", "strengths");
            migrationBuilder.RenameColumn("UserId", "HR_Review", "user_id");
            migrationBuilder.RenameColumn("TrainingRecommendations", "HR_Review", "training_recommendations");
            migrationBuilder.RenameColumn("TeamworkRating", "HR_Review", "teamwork_rating");
            migrationBuilder.RenameColumn("OverallRating", "HR_Review", "overall_rating");
            migrationBuilder.RenameColumn("LeadershipRating", "HR_Review", "leadership_rating");
            migrationBuilder.RenameColumn("InterviewId", "HR_Review", "interview_id");
            migrationBuilder.RenameColumn("CommunicationRating", "HR_Review", "communication_rating");
            migrationBuilder.RenameColumn("CareerPathNotes", "HR_Review", "career_path_notes");
            migrationBuilder.RenameColumn("AreasForImprovement", "HR_Review", "areas_for_improvement");
            migrationBuilder.RenameColumn("AdaptabilityRating", "HR_Review", "adaptability_rating");
            migrationBuilder.RenameColumn("ReviewId", "HR_Review", "review_id");

            // ---------- Drop Indexes ----------
            migrationBuilder.DropIndex("IX_HR_Review_UserId", "HR_Review");
            migrationBuilder.DropIndex("IX_HR_Review_InterviewId", "HR_Review");

            // ---------- Employee_Records ----------
            migrationBuilder.RenameColumn("OfferLetterPath", "Employee_Records", "offer_letter_path");
            migrationBuilder.RenameColumn("JoiningDate", "Employee_Records", "joining_date");
            migrationBuilder.RenameColumn("JobId", "Employee_Records", "job_id");
            migrationBuilder.RenameColumn("CandidateId", "Employee_Records", "candidate_id");
            migrationBuilder.RenameColumn("EmployeeId", "Employee_Records", "employee_id");

            // ---------- Drop Index ----------
            migrationBuilder.DropIndex("IX_Employee_Records_JobId", "Employee_Records");

            // ---------- EmailRecipients ----------
            migrationBuilder.RenameColumn("Email", "EmailRecipients", "email");
        }
    }
}
