using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.HangFireJobs;
using RecruitmentApi.Models;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailService _service;
        private readonly AppDbContext _db;
        public EmailController(AppDbContext db, EmailService service)
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("sofiamarshal77@gmail.com", "ghpz oedu mwfi oovq"),
                EnableSsl = true,
            };

            _db = db;
            _service = service;
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

        public class EmailSchedulerRequest
        {
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public List<string> ToUserIds { get; set; } = new();
            public List<string> CcUserIds { get; set; } = new();

            public DateTime ScheduledAt { get; set; }
        }

        [HttpPost("EmailScheduler")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> SendEmail([FromBody] EmailSchedulerRequest request)
        {
            var email = new EmailMessage
            {
                Subject = request.Subject,
                Body = request.Body,
                CreatedAt = DateTime.UtcNow,
                ScheduledAt = request.ScheduledAt,
                Recipients = new List<EmailRecipient>()
            };

            request.ToUserIds.ForEach(id =>
                email.Recipients.Add(new EmailRecipient
                {
                    user_id = id,
                    Type = RecipientType.To
                }));

            request.CcUserIds.ForEach(id =>
                email.Recipients.Add(new EmailRecipient
                {
                    user_id = id,
                    Type = RecipientType.Cc
                }));

            _db.EmailMessages.Add(email);
            await _db.SaveChangesAsync();

            BackgroundJob.Schedule<IEmailJob>(
                job => job.SendEmail(email.Id),
                request.ScheduledAt);

            return Ok(new
            {
                success = true,
                message = "Email successfully scheduled.",
                data=true
            });
        }

        [HttpGet("GetAllMails")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> GetAllMails()
        {
            try
            {
                var res = await _service.GetEmailsAsync();
                return Ok(new
                {
                    success = true,
                    message = "All Emails fetched successfully",
                    data = res
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An Error occured while fetching all mails",
                    error = ex.Message
                });
            }
        }

        [HttpPut("UpdateEmail")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> UpdateEmail([FromBody] EmailDtos.UpdateReq req)
        {
            try
            {
                await _service.UpdateEmailAsync(req);
                return Ok(new
                {
                    success = true,
                    message = "Email updated successfully",
                    data = true
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An Error occured while updating email",
                    error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An Error occured while updating email",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An Error occured while updating email",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("DeleteEmail/{Id}")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> DeleteEmail(int Id)
        {
            try
            {
                await _service.DeleteEmailAsync(Id);
                return Ok(new
                {
                    success = true,
                    message = "All Emails fetched successfully",
                    data = true
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An Error occured while fetching all mails",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An Error occured while fetching all mails",
                    error = ex.Message
                });
            }
        }

    }
}

