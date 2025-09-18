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

    //Dto to get login data
    public class LoginData
    {
        public string Id { get; set; } = null!;
        public string Password { get; set; } = null!;
        public List<Role> Roles { get; set; } = new();
    }

    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public JwtService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        //Method to generate token
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

        //Get all UserId, All Roles and Hashed password
        public async Task<LoginData?> GetLoginData(string email)
        {
            var user = await _context.Users
                .Include(u => u.roles)
                .FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
                return null;

            return new LoginData
            {
                Id = user.user_id,
                Password = user.password_hash,
                Roles = user.roles.ToList()
            };
        }

    }

}
