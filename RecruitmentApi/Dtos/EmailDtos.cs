namespace RecruitmentApi.Dtos
{
    public class EmailDtos
    {
        public class UpdateReq
        {
            public int Id { get; set; }

            public string Subject { get; set; }
            public string Body { get; set; }

            public DateTime ScheduledAt { get; set; }
        }
    }
}
