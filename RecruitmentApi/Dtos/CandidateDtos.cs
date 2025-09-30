﻿using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    public class CandidateDtos
    {
        public class CandidateDto
        {
            public string candidate_id { get; set; } = null!;

            public string full_name { get; set; } = null!;

            public string email { get; set; } = null!;
        }

        public class CandidateListDto : CandidateDto
        {
            public string? phone { get; set; }

            public DateTime created_at { get; set; }
        }

        public class CreateCandidateDto : CandidateDto
        {
            public string? phone { get; set; }

            public string resume_path { get; set; } = null!;

            public string password { get; set; } = null!;
        }

        public class UpdateCandidateDto : CandidateDto
        {
            public string? phone { get; set; }
            public string? resume_path { get; set; } = null!;
        }

        public class DeleteCandidateDto
        {
            public string candidate_id { get; set; } = null!;
        }

        // In CandidateDtos.cs

        public class CandidateProfileDto : CandidateDto
        {
            public string? phone { get; set; }
            public string resume_path { get; set; } = null!;
            public DateTime created_at { get; set; }

            // --- CRITICAL FIX ---
            // These collections MUST use DTOs, not database models.
            public virtual ICollection<Candidate_DocumentDtos.Candidate_DocumentDto> Candidate_Documents { get; set; } = new List<Candidate_DocumentDtos.Candidate_DocumentDto>();
            public virtual ICollection<Candidate_ReviewDtos.CandidateReviewDto_Candidate> Candidate_Reviews { get; set; } = new List<Candidate_ReviewDtos.CandidateReviewDto_Candidate>();
            public virtual ICollection<Candidate_SkillDtos.Candidate_SkillDto> Candidate_Skills { get; set; } = new List<Candidate_SkillDtos.Candidate_SkillDto>();
            public virtual ICollection<Candidate_Status_HistoryDtos.Candidate_Status_HistoryDto_Candidate> Candidate_Status_Histories { get; set; } = new List<Candidate_Status_HistoryDtos.Candidate_Status_HistoryDto_Candidate>();
            public virtual Employee_RecordDtos.Employee_RecordDto_Candidate? Employee_Record { get; set; }
            public virtual ICollection<InterviewDtos.InterviewDtos_Candidate> Interviews { get; set; } = new List<InterviewDtos.InterviewDtos_Candidate>();
        }

    }
}
