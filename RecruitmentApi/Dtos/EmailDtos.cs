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

        public class EmailSchedulerRequest
        {
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public List<string> ToUserIds { get; set; } = new();
            public List<string> CcUserIds { get; set; } = new();

            public DateTime ScheduledAt { get; set; }
        }

        public class AutoMailReq
        {
            public int JobId { get; set; }
            public string Body { get; set; } = null!;
            public string Subject { get; set; } = null!;

            public List<string> To { get; set; } = new();

            public List<string> Cc { get; set; } = new();
            public DateTime scheduledAt { get; set; }
        }
    }
}
