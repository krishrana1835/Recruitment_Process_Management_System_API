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
        /// <summary>
        /// Gets all available roles.
        /// </summary>
        /// <returns>A list of all available roles.</returns>
        /// <response code="200">Returns a list of RoleDto objects.</response>
        [HttpGet]
        public async Task<ActionResult<List<RoleDtos.RoleDto>>> getRoles()
        {
            var Results = await _roleService.GetRolesAsync();
            return Ok(Results);
        }

        //Get all roles of specific user
        /// <summary>
        /// Gets all roles associated with a specific user email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>A list of roles for the specified user.</returns>
        /// <response code="200">Returns a list of RoleDto objects for the user.</response>
        /// <response code="404">If no roles are found for the specified user.</response>
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
