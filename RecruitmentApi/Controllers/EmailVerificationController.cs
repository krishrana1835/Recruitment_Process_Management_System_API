using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        // Temporary in-memory store for OTPs (use Redis or DB in production)
        private static readonly ConcurrentDictionary<string, OtpEntry> _otpStore = new();

        private readonly CandidateService _candidateService;

        private readonly SmtpClient _smtpClient;

        private readonly Random _random = new();

        public EmailVerificationController(CandidateService candidateService)
        {
            // Configure SMTP client (example uses Gmail SMTP)
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("sofiamarshal77@gmail.com", "fqsc rtvs tmoq svey"),
                EnableSsl = true,
            };

            _candidateService = candidateService;
        }

        // Generate OTP
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Identifier))
                return BadRequest("Identifier (email/phone) is required.");

            // Generate random 6-digit OTP
            string otp = _random.Next(100000, 999999).ToString();

            // Store OTP with 5-minute expiry
            _otpStore[request.Identifier] = new OtpEntry
            {
                Otp = otp,
                Expiry = DateTime.UtcNow.AddMinutes(5)
            };

            if (string.IsNullOrEmpty(request.Identifier))
                return BadRequest("Recipient email is required.");

            if(await _candidateService.IsRegisteredAsync(request.Identifier))
                return StatusCode(500, "Email is already in use");

            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("sofiamarshal77@gmail.com"),
                    Subject = "OTP for Roima Registration",
                    Body = $"Yor OTP is '{otp}', Please do not share with anyone.",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(request.Identifier);

                await _smtpClient.SendMailAsync(mailMessage);

                return Ok(new { message = "OTP generated and sent successfully.", success = true });
            }
            catch (SmtpException ex)
            {
                // Log exception (ex)
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }

        // Verify OTP
        [HttpPost("verify")]
        public IActionResult VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            if (!_otpStore.TryGetValue(request.Identifier, out var entry))
                return BadRequest("OTP not found or expired.");

            if (DateTime.UtcNow > entry.Expiry)
            {
                _otpStore.TryRemove(request.Identifier, out _);
                return BadRequest("OTP expired.");
            }

            if (entry.Otp != request.Otp)
                return BadRequest("Invalid OTP.");

            // OTP is valid — remove it to prevent reuse
            _otpStore.TryRemove(request.Identifier, out _);

            return Ok(new { message = "OTP verified successfully!", verified = true });
        }
    }

    // Request models
    public class OtpRequest
    {
        public string Identifier { get; set; } = null!;
    }

    public class OtpVerifyRequest
    {
        public string Identifier { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }

    // OTP storage model
    public class OtpEntry
    {
        public string Otp { get; set; } = null!;
        public DateTime Expiry { get; set; }

    }
}
