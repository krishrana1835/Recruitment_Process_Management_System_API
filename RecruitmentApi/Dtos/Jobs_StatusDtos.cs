namespace RecruitmentApi.Dtos
{
    public class Jobs_StatusDtos
    {
        public class Jobs_StatusDto
        {
            public int status_id { get; set; }

            public string status { get; set; } = null!;

            public string? reason { get; set; }

            public DateTime changed_at { get; set; }

            public string changed_by { get; set; } = null!;
        }
    }
}
