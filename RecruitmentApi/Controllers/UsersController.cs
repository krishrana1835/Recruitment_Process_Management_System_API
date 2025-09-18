using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _userService;
        public UsersController(UsersService usersService)
        {
            _userService = usersService;
        }

        //Get all users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User?>> getAllUsers()
        {
            var users = await _userService.GetUserAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User?>> getUser(string id)
        {
            var users = await _userService.GetUserAsync(id);
            return Ok(users);
        }

        [HttpGet("GetUserInfo")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDtos.UserCreateDto?>> getAllUsersSmallData()
        {
            var users = await _userService.GetAllUserInfoAsync();
            return Ok(users);
        }

        [HttpGet("GetUserProfile/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDtos.UserProfileDto?>> getUserProfile(string id)
        {
            if (id == null)
                return BadRequest("Plese enter user_id");
            var user = await _userService.GetUserProfileAsync(id);
            if(user == null)
                return NotFound("No user found with user_id "+id);
            return Ok(user);
        }

        //Add new user
        //Uses UserCreateDto and returns UserDto
        [HttpPost]
        public async Task<ActionResult<UserDtos.UserDto>> addUser(UserDtos.UserCreateDto newUser)
        {
            //Check Null input
            if (newUser == null)
                return BadRequest("Input data required");

            //Check if user is already there
            var check = await _userService.GetUserAsync(newUser.user_id);
            if (check != null)
                return Conflict("User Already exist");

            //Making object of user
            User user = new User
            {
                user_id = newUser.user_id,
                name = newUser.name,
                email = newUser.email,
                password_hash = BCrypt.Net.BCrypt.HashPassword(newUser.password),
                created_at = DateTime.Now,
            };

            //Adding User
            var res = await _userService.AddUserAsync(user);

            return CreatedAtAction(nameof(addUser), new { user.email }, res);
        }
    }
}
