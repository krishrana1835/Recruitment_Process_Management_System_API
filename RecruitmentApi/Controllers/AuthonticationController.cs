using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace RecruitmentApi.Controllers
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using RecruitmentApi.Services;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
        {

            //Input validation
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                return BadRequest("Please enter valid email and password");

            if (string.IsNullOrWhiteSpace(login.Role))
                return BadRequest("Please select role");

            //Fetch Password, role and id
            var logindata = await _jwtService.GetLoginData(login.Email);

            // User Exist or not
            if (logindata == null)
                return Unauthorized("User not found");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(login.Password, logindata.Password))
                return Unauthorized("Invalid password");

            // Check if user has the selected role
            if (!logindata.Roles.Any(r => r.role_name == login.Role))
                return Unauthorized("User does not have the selected role");

            //Generate token
            var token = _jwtService.GenerateToken(login.Email, login.Role);

            // Return success response
            return Ok(new
            {
                token,
                user = new
                {
                    UserId = logindata.Id,
                    Email = login.Email,
                    Role = login.Role
                }
            });
        }
    }


    //User Login DTO
    public class UserLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }

}
