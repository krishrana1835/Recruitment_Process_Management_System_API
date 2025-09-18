using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        //Get all ther availabel roles
        [HttpGet]
        public async Task<ActionResult<List<RoleDtos.RoleDto>>> getRoles()
        {
            var Results = await _roleService.GetRolesAsync();
            return Ok(Results);
        }

        //Get all roles of specific user
        [HttpGet("email")]
        public async Task<ActionResult<List<RoleDtos.RoleDto>>> getRoles(string email)
        {
            var results = await _roleService.GetRolesAsync(email);
            if (results.Count == 0)
                return NotFound();
            return Ok(results);
        }
    }
}
