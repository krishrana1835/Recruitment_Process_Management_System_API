using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;

namespace RecruitmentApi.Services
{
    /// <summary>
    /// Service for managing roles.
    /// </summary>
    public class RoleService
    {
        private AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all available roles.
        /// </summary>
        /// <returns>A list of <see cref="RoleDtos.RoleDto"/> objects representing all available roles.</returns>
        public async Task<List<RoleDtos.RoleDto>> GetRolesAsync()
        {
            //Select required columns and returning it as RoleDto
            var data = await _context.Roles.ToListAsync();
            var roles = data.Select(r => new RoleDtos.RoleDto
            {
                role_id = r.RoleId,
                role_name = r.RoleName
            }).ToList();
            return roles;
        }

        /// <summary>
        /// Gets a list of all roles for a specific user.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>A list of <see cref="RoleDtos.RoleDto"/> objects representing the user's roles, or null if the user or their roles are not found.</returns>
        public async Task<List<RoleDtos.RoleDto?>> GetRolesAsync(string email)
        {
            //Fetching All roles of user
            var user = await _context.Users.Include(r => r.Roles).FirstOrDefaultAsync(u => u.Email == email);

            //User not available
            if (user == null)
                return null;

            //User do not have any role
            if (user.Roles == null)
                return null;

            //Converting Roles into List of RoleDto
            var roles = user.Roles.Select(r => new RoleDtos.RoleDto
            {
                role_id = r.RoleId,
                role_name = r.RoleName,
            }).ToList();

            return roles;
        }
    }
}