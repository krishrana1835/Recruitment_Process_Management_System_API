using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    public class Candidate_Status_HistoryDtos
    {
        public class Candidate_Status_HistoryDto_Candidate
        {
            public int candidate_status_id { get; set; }

            public string status { get; set; } = null!;

            public string? reason { get; set; }

            public DateTime changed_at { get; set; }

            public JobDtos.JobTitle job { get; set; } = null!;
        }

        public class JobApplicationByCandidate
        {
            public string candidate_id { get; set; } = null!;

            public int job_id { get; set; }
        }

        public class UpdateCandidateStatusRequest
        {
            public string candidate_id { get; set; } = null!;

            public string status { get; set; } = null!;

            public int job_id { get; set; }

            public string? reason { get; set; }

            public string changed_by { get; set; } = null!;
        }

        public class JobApplicationStatus
        {
            public int candidate_status_id { get; set; }
            public string status { get; set; } = null!;
            public DateTime changed_at { get; set; }
            public string? reason { get; set; }
            public virtual JobDtos.ListJobStatus job { get; set; } = null!;
        }
    }
}
