namespace RecruitmentApi.Models
{
    public class EmailMessage
    {
        public int Id { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime ScheduledAt { get; set; }
        public ICollection<EmailRecipient> Recipients { get; set; }
    }
}
