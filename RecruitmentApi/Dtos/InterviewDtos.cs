namespace RecruitmentApi.Dtos
{
    public class InterviewDtos
    {
        public class InterviewDtos_Candidate
        {
            public int interview_id { get; set; }
            public int job_id { get; set; }

            public JobDtos.JobDto_Candidate job { get; set; }
        }
    }
}
