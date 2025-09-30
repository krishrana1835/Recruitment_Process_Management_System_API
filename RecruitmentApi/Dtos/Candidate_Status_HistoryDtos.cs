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
        }
    }
}
