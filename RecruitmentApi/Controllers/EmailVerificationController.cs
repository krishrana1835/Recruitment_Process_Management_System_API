using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecruitmentApi.Data;
using RecruitmentApi.Models;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailVerificationController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, OtpEntry> _otpStore = new();
        private static readonly ConcurrentDictionary<string, ResetPasswordStore> _passwordReset = new();

        private readonly CandidateService _candidateService;
        private readonly EmailConfigurationModel _emailSettings;
        private readonly Random _random = new();
        private readonly AppDbContext _context;

        public EmailVerificationController(CandidateService candidateService,AppDbContext context,IOptions<EmailConfigurationModel> emailOptions)
        {
            _candidateService = candidateService;
            _emailSettings = emailOptions.Value;
            _context = context;
        }
        private static string HashToken(string token, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }

        // Generate OTP
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identifier))
                return BadRequest("Email is required.");

            if (await _candidateService.IsRegisteredAsync(request.Identifier))
                return Conflict("Email is already in use.");

            string otp = _random.Next(100000, 999999).ToString();

            _otpStore[request.Identifier] = new OtpEntry
            {
                Otp = otp,
                Expiry = DateTime.UtcNow.AddMinutes(5)
            };

            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(
                        _emailSettings.Username,
                        _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.From),
                    Subject = "OTP for Roima Registration",
                    Body = $"Your OTP is <b>{otp}</b>. Please do not share it with anyone.",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(request.Identifier);

                await smtpClient.SendMailAsync(mailMessage);

                return Ok(new { success = true, message = "OTP generated and sent successfully." });
            }
            catch (SmtpException ex)
            {
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

            _otpStore.TryRemove(request.Identifier, out _);

            return Ok(new { verified = true, message = "OTP verified successfully!" });
        }


        [HttpPost("generate-fpassword-otp")]
        public async Task<IActionResult> GenerateForgotpasswordOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identifier))
                return BadRequest(new
                {
                    success = false,
                    message = "Email is required.",
                    error = "Email is required."
                });

            var check = await _context.Candidates.AnyAsync(i => i.Email == request.Identifier) || await _context.Users.AnyAsync(i => i.Email == request.Identifier);

            if (!check)
                return BadRequest(new
                {
                    success = false,
                    message = "User does not exist",
                    error = "User does not exist"
                });

            string otp = _random.Next(100000, 999999).ToString();

            _otpStore[request.Identifier] = new OtpEntry
            {
                Otp = otp,
                Expiry = DateTime.UtcNow.AddMinutes(5)
            };

            try
            {
                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(
                        _emailSettings.Username,
                        _emailSettings.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.From),
                    Subject = "OTP for Roima Forgot Password",
                    Body = $"Your OTP is <b>{otp}</b>. Please do not share it with anyone.",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(request.Identifier);

                await smtpClient.SendMailAsync(mailMessage);

                return Ok(new { success = true, message = "OTP generated and sent successfully." });
            }
            catch (SmtpException ex)
            {
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }

        [HttpPost("verify-fpassword-otp")]
        public IActionResult VerifyForgotpasswordOtp([FromBody] OtpVerifyRequest request)
        {
            if (!_otpStore.TryGetValue(request.Identifier, out var entry))
                return BadRequest(new
                {
                    success = false,
                    message = "OTP not found or expired.",
                    error = "OTP not found or expired."
                });

            if (DateTime.UtcNow > entry.Expiry)
            {
                _otpStore.TryRemove(request.Identifier, out _);
                return BadRequest(new
                {
                    success = false,
                    message = "OTP expired.",
                    error = "OTP expired."
                });
            }

            if (entry.Otp != request.Otp)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid OTP.",
                    error = "Invalid OTP."
                });

            _otpStore.TryRemove(request.Identifier, out _);

            string rawToken = _random.Next(100000, 999999).ToString();
            string hashedToken = HashToken(rawToken,"ThisIsMySecerateKey!!!");

            _passwordReset[request.Identifier] = new ResetPasswordStore
            {
                ResetToken = hashedToken,
                Expiry = DateTime.UtcNow.AddMinutes(5)
            };

            return Ok(new
            {
                success = true,
                message = "Otp verified successfully",
                data = new
                {
                    verified = true,
                    resetToken = rawToken
                }
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!_passwordReset.TryGetValue(request.Identifier, out var entry))
                return BadRequest(new
                {
                    success = false,
                    message = "Reset Token not found or expired.",
                    error = "Reset Token not found or expired."
                });

            if (DateTime.UtcNow > entry.Expiry)
            {
                _otpStore.TryRemove(request.Identifier, out _);
                return BadRequest(
                    new
                    {
                        success = false,
                        message = "Session Token expired.",
                        error = "Session Token expired."
                    });
            }

            if (HashToken(request.ResetToken, "ThisIsMySecerateKey!!!") != entry.ResetToken)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Session.",
                    error = "Invalid Session."
                });

            _passwordReset.TryRemove(request.Identifier, out _);

            var user = await _context.Users.FirstOrDefaultAsync(i => i.Email == request.Identifier);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            }

            var candidate = await _context.Candidates.FirstOrDefaultAsync(i => i.Email == request.Identifier);
            if (candidate != null)
            {
                candidate.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Password Reset",
                data = true
            });
        }
    }

    // DTOs
    public class OtpRequest
    {
        public string Identifier { get; set; } = null!;
    }

    public class OtpVerifyRequest
    {
        public string Identifier { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }

    public class ResetPasswordStore
    {
        public string ResetToken { get; set; } = null!;
        public DateTime Expiry { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Identifier { get; set; } = null!;
        public string ResetToken { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class OtpEntry
    {
        public string Otp { get; set; } = null!;
        public DateTime Expiry { get; set; }
    }
}
