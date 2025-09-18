using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;

namespace RecruitmentApi.Services
{
    public class RoleService
    {
        private AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        //Get RoleId and RoleName of all available roles
        public async Task<List<RoleDtos.RoleDto>> GetRolesAsync()
        {
            //Select required columns and returning it as RoleDto
            var data = await _context.Roles.ToListAsync();
            var roles = data.Select(r => new RoleDtos.RoleDto
            {
                role_id = r.role_id,
                role_name = r.role_name
            }).ToList();
            return roles;
        }

        //Get List of all roles of Specific user
        public async Task<List<RoleDtos.RoleDto?>> GetRolesAsync(string email)
        {
            //Fetching All roles of user
            var user = await _context.Users.Include(r => r.roles).FirstOrDefaultAsync(u => u.email == email);

            //User not available
            if (user == null)
                return null;

            //User do not have any role
            if (user.roles == null)
                return null;

            //Converting Roles into List of RoleDto
            var roles = user.roles.Select(r => new RoleDtos.RoleDto
            {
                role_id = r.role_id,
                role_name = r.role_name,
            }).ToList();

            return roles;
        }
    }
}
