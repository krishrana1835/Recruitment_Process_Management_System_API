using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RecruitmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly SmtpClient _smtpClient;

        public EmailController()
        {
            // Configure SMTP client (example uses Gmail SMTP)
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("sofiamarshal77@gmail.com", "fqsc rtvs tmoq svey"),
                EnableSsl = true,
            };
        }

        public class EmailRequest
        {
            public string ToEmail { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin, Candidate, Reviewer")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.ToEmail))
                return BadRequest("Recipient email is required.");

            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("sofiamarshal77@gmail.com"),
                    Subject = request.Subject,
                    Body = request.Body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(request.ToEmail);

                await _smtpClient.SendMailAsync(mailMessage);

                return Ok(new {message = "Email sent successfully." });
            }
            catch (SmtpException ex)
            {
                // Log exception (ex)
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }
    }
}

