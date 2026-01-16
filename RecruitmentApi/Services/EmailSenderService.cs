using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.HangFireJobs;
using RecruitmentApi.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;


namespace RecruitmentApi.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(
            List<string> to,
            List<string> cc,
            string subject,
            string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Email:From"]));

            to.ForEach(x => message.To.Add(MailboxAddress.Parse(x)));
            cc.ForEach(x => message.Cc.Add(MailboxAddress.Parse(x)));

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _config["Email:Username"],
                _config["Email:Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        
    }

    public class EmailService
    {
        private readonly AppDbContext _context;

        public EmailService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmailMessage>> GetEmailsAsync()
        {
            var emails = await _context.EmailMessages.ToListAsync();

            return emails;
        }

        

        public async Task<EmailDtos.UpdateReq> UpdateEmailAsync(EmailDtos.UpdateReq req)
        {
            var email = await _context.EmailMessages.FirstOrDefaultAsync(i => i.Id == req.Id);
            if (email == null)
                throw new NullReferenceException("Email not found");

            if (email.IsSent == true)
                throw new InvalidOperationException("Can not update email which is already sent");
            email.Subject = req.Subject;
            email.Body = req.Body;
            email.ScheduledAt = req.ScheduledAt;

            await _context.SaveChangesAsync();
            return req;
        }

        public async Task DeleteEmailAsync(int id)
        {
            var email = await _context.EmailMessages.FirstOrDefaultAsync( i=> i.Id == id);
            if (email == null)
                throw new NullReferenceException("Email does not exist");

            var recipents = await _context.EmailRecipients.Where(i => i.EmailMessageId == id).ToListAsync();

            _context.EmailMessages.Remove(email);
            _context.RemoveRange(recipents);

            await _context.SaveChangesAsync();
        }
        public class Recipients
        {
            public string email { get; set; } = null!;
            public RecipientType type { get; set; }
        }
        public async Task<List<Recipients>> GetRecipientsAsync(int id)
        {
            var email = await _context.EmailRecipients.Where(r => r.EmailMessageId == id)
                .Select(r => new Recipients
                {
                    email = r.Email,
                    type = r.Type
                }).ToListAsync();
            return email;
        }
    }

}
