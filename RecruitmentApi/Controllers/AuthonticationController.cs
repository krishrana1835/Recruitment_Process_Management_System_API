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

        /// <summary>
        /// Authenticates a user and returns a JWT token upon successful login.
        /// </summary>
        /// <param name="login">User login credentials including email, password, and role.</param>
        /// <returns>An IActionResult containing the JWT token and user information if authentication is successful.</returns>
        /// <response code="200">Returns the JWT token and user details.</response>
        /// <response code="400">If the email, password, or role is invalid or missing.</response>
        /// <response code="401">If the user is not found, password is invalid, or user does not have the specified role.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
        {

            //Input validation
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
                return BadRequest("Please enter valid email and password");

            if (string.IsNullOrWhiteSpace(login.Role))
                return BadRequest("Please select role");

            //Fetch Password, role and id
            var logindata = await _jwtService.GetLoginData(login.Email, login.Role);

            // User Exist or not
            if (logindata == null)
                return Unauthorized("User not found");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(login.Password, logindata.Password))
                return Unauthorized("Invalid password");

            // Check if user has the selected role
            if (!logindata.Roles.Any(r => r.RoleName == login.Role))
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
    /// <summary>
    /// Represents the data transfer object for user login.
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public string Role { get; set; } = null!;
    }

}
