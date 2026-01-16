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

        public string Email { get; set; } = null!;

        public RecipientType Type { get; set; }
    }
}
