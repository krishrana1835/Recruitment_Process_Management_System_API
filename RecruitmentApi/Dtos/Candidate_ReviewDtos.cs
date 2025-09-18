namespace RecruitmentApi.Dtos
{
    public class Candidate_ReviewDtos
    {
        public class Candidate_ReviewDto
        {
            public int review_id { get; set; }

            public string comments { get; set; } = null!;

            public DateTime reviewed_at { get; set; }

            public string candidate_id { get; set; } = null!;

            public int job_id { get; set; }
        }
    }
}
