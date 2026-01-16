using RecruitmentApi.Data;

namespace RecruitmentApi.Dtos
{
    public class ReportsDtos
    {
        public class InterviewSummaryRequest
        {
            public string UserId { get; set; } = null!;
            public int? JobId { get; set; }
            public int? RoundNumber { get; set; }
            public bool IncludeCandidateInfo { get; set; } = true;
            public DateTime? InterviewDate { get; set; }
        }

        public class InterviewSummaryResponse
        {
            public int JobId { get; set; }
            public int TotalCandidates { get; set; }
            public int SelectedCandidates { get; set; }
            public double AverageScore { get; set; }
            public DateTime LastInterviewDate { get; set; }
            public List<CandidateSummaryDto> SelectedCandidateDetails { get; set; } = new();
        }

        public class CandidateSummaryDto
        {
            public string CandidateId { get; set; } = null!;
            public string CandidateName { get; set; } = null!;
            public string? Email { get; set; }
        }

        public class TechReq
        {
            public int JobId { get; set; }
            public List<int> SkillId { get; set; } = new();

        }

        public class CandidateDto
        {
            public string candidate_id { get; set; } = null!;

            public string full_name { get; set; } = null!;

            public string email { get; set; } = null!;

            public float totalExpirence { get; set; }
        }

        public class ExpirienceReq
        {
            public int JobId { get; set; }
            public List<int> SkillId { get; set; } = new();
            public int MinExp { get; set; } = 0;
        }

        public class DailySummaryDto
        {
            public DateTime ReportDate { get; set; }

            // 1. Candidate Metrics
            public int TotalCandidatesAdded { get; set; }
            public List<CandidateSummaryDto> CandidatesAdded { get; set; } = new();

            // 2. Email Metrics
            public List<EmailSummaryDto> EmailsSent { get; set; } = new();

            // 3. Employee Metrics
            public List<EmployeeSummaryDto> NewEmployees { get; set; } = new();

            // 4. Interview Metrics
            public InterviewMetricsDto InterviewStats { get; set; } = new();

            // 5. Job Metrics
            public List<JobSummaryDto> JobsCreated { get; set; } = new();
            public List<JobSummaryDto> JobsStatusChanged { get; set; } = new();

            // 6. User Metrics
            public List<UserSummaryDto> UsersCreated { get; set; } = new();
        }

        //public class CandidateSummaryDto
        //{
        //    public string CandidateId { get; set; }
        //    public string FullName { get; set; }
        //    public string Email { get; set; }
        //}

        public class EmailSummaryDto
        {
            public int MessageId { get; set; }
            public string Subject { get; set; }
            public List<string> Recipients { get; set; } = new();
            public DateTime SentAt { get; set; }
        }

        public class EmployeeSummaryDto
        {
            public int EmployeeId { get; set; }
            public string CandidateId { get; set; }
            public string CandidateName { get; set; }
            public string CandidateEmail { get; set; }
            public int JobId { get; set; }
            public string JobTitle { get; set; }
        }

        public class InterviewMetricsDto
        {
            public int TotalInterviewsTaken { get; set; }

            // Breakdown
            public int TotalHrInterviews { get; set; }
            public int TotalSimpleInterviews { get; set; } // Technical/Normal

            // Selections
            public int SelectedInHr { get; set; }
            public int SelectedInSimple { get; set; }
        }

        public class JobSummaryDto
        {
            public int JobId { get; set; }
            public string JobTitle { get; set; }
            public string Status { get; set; }
        }

        public class UserSummaryDto
        {
            public string UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public List<RoleDto> Roles { get; set; } = new();
        }

        public class RoleDto
        {
            public string RoleName { get; set; } // Assuming Role entity has Name
        }

        public class CandidateSummaryReportDto
        {
            // Basic Info
            public string CandidateId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string ResumePath { get; set; }

            // Documents
            public List<CandidateDocumentDto> Documents { get; set; }

            // Status History (Grouped by Job)
            public List<JobStatusHistoryGroupDto> JobStatusHistories { get; set; }

            // Interviews & Feedback (Grouped by Job -> Round)
            public List<JobInterviewSummaryDto> JobInterviews { get; set; }

            // Employee Record (Nullable)
            public EmployeeRecordDto? EmployeeRecord { get; set; }
        }

        public class CandidateDocumentDto
        {
            public int DocumentId { get; set; }
            public string DocumentType { get; set; }
            public string FilePath { get; set; }
            public string VerificationStatus { get; set; }
            public DateTime UploadedAt { get; set; }
        }

        // --- Status Grouping ---
        public class JobStatusHistoryGroupDto
        {
            public int JobId { get; set; }
            public string JobTitle { get; set; }
            public List<StatusHistoryDto> History { get; set; }
        }

        public class StatusHistoryDto
        {
            public string Status { get; set; }
            public string Reason { get; set; }
            public DateTime ChangedAt { get; set; }
            public string ChangedBy { get; set; }
        }

        // --- Interview Grouping ---
        public class JobInterviewSummaryDto
        {
            public int JobId { get; set; }
            public string JobTitle { get; set; }
            public List<InterviewRoundDto> Rounds { get; set; }
        }

        public class InterviewRoundDto
        {
            public int RoundNumber { get; set; }

            public string RoundTitle { get; set; }
            public List<SkillFeedbackDto> SkillFeedbacks { get; set; }
            public List<HrReviewDto> HrReviews { get; set; }
        }

        public class SkillFeedbackDto
        {
            public int InterviewId { get; set; }
            public string SkillName { get; set; }
            public int TechnicalRating { get; set; }
            public int ConceptRating { get; set; }
            public string Comments { get; set; }
            public string FeedbackByUserId { get; set; }
        }

        public class HrReviewDto
        {
            public int InterviewId { get; set; }
            public int CommunicationRating { get; set; }
            public int OverallRating { get; set; }
            public string Strengths { get; set; }
            public string AreasForImprovement { get; set; }
            public string ReviewByUserId { get; set; }
            public int TeamworkRating { get; set; }
            public int AdaptabilityRating { get; set; }
            public int LeadershipRating { get; set; }
            public string TrainingRecommendations { get; set; } = null!;
            public string CareerPathNotes { get; set; } = null!;
        }

        public class EmployeeRecordDto
        {
            public int EmployeeId { get; set; }
            public DateOnly JoiningDate { get; set; }
            public string OfferLetterPath { get; set; }
            public int JobId { get; set; }
            public string JobTitle { get; set; }
        }
    }
}
