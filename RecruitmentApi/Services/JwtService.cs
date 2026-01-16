namespace RecruitmentApi.Services
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using RecruitmentApi.Data;
    using RecruitmentApi.Models;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// Represents the data required for user login, including ID, hashed password, and roles.
    /// </summary>
    public class LoginData
    {
        /// <summary>
        /// Gets or sets the user's unique identifier.
        /// </summary>
        public string Id { get; set; } = null!;
        /// <summary>
        /// Gets or sets the user's hashed password.
        /// </summary>
        public string Password { get; set; } = null!;
        /// <summary>
        /// Gets or sets the list of roles assigned to the user.
        /// </summary>
        public List<Role> Roles { get; set; } = new();
    }

    /// <summary>
    /// Service for generating and validating JSON Web Tokens (JWT).
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        /// <param name="config">The application's configuration.</param>
        /// <param name="context">The application's database context.</param>
        public JwtService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        /// <summary>
        /// Generates a JWT token for the specified email and role.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="role">The user's role.</param>
        /// <returns>A JWT token string.</returns>
        public string GenerateToken(string email, string role)
        {
            //adding claims
            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Generating roken using creds
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Retrieves login data (user ID, hashed password, and roles) for a given email.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>A <see cref="LoginData"/> object if the user is found; otherwise, null.</returns>
        public async Task<LoginData?> GetLoginData(string email, string role)
        {
            switch (role)
            {
                case "Candidate":
                    var candidate = await _context.Candidates
                        .FirstOrDefaultAsync(c => c.Email == email);

                    if (candidate == null)
                        return null;

                    return new LoginData
                    {
                        Id = candidate.CandidateId,
                        Password = candidate.Password,
                        Roles = new List<Role> { new Role { RoleName = "Candidate" } } // Assuming Role class
                    };

                case "Admin":
                case "Recruiter":
                case "HR":
                case "Interviewer":
                case "Reviewer":
                case "Viewer":
                    var user = await _context.Users
                        .Include(u => u.Roles)
                        .FirstOrDefaultAsync(u => u.Email == email);

                    if (user == null)
                        return null;

                    return new LoginData
                    {
                        Id = user.UserId,
                        Password = user.PasswordHash,
                        Roles = user.Roles.ToList()
                    };

                default:
                    return null; // Role not recognized
            }
        }


    }

}