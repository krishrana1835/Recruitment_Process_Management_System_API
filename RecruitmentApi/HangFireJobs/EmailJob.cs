using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Data;
using RecruitmentApi.Models;

namespace RecruitmentApi.HangFireJobs
{
    public interface IEmailJob
    {
        Task SendEmail(int emailMessageId);
    }

    public interface IEmailSender
    {
        Task SendAsync(
            List<string> to,
            List<string> cc,
            string subject,
            string body);
    }

    public class EmailJob : IEmailJob
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;

        public EmailJob(AppDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public async Task SendEmail(int emailMessageId)
        {
            var email = await _db.EmailMessages
                .Include(e => e.Recipients)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(e => e.Id == emailMessageId);

            if (DateTime.UtcNow < email.ScheduledAt)
                return;

            if (email == null || email.IsSent)
                return;

            var toEmails = email.Recipients
                .Where(r => r.Type == RecipientType.To)
                .Select(r => r.User.email)
                .ToList();

            var ccEmails = email.Recipients
                .Where(r => r.Type == RecipientType.Cc)
                .Select(r => r.User.email)
                .ToList();

            await _emailSender.SendAsync(
                toEmails,
                ccEmails,
                email.Subject,
                email.Body);

            email.IsSent = true;
            await _db.SaveChangesAsync();
        }
    }

}
