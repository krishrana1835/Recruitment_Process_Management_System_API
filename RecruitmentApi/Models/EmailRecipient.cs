namespace RecruitmentApi.Models
{
    public enum RecipientType
    {
        To = 1,
        Cc = 2
    }


    public class EmailRecipient
    {
        public int Id { get; set; }

        public int EmailMessageId { get; set; }
        public EmailMessage EmailMessage { get; set; }

        public string user_id { get; set; } = null!;
        public User User { get; set; }

        public RecipientType Type { get; set; }
    }
}
