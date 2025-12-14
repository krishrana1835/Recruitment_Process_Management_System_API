using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;
using RecruitmentApi.Services;
using static RecruitmentApi.Dtos.UserDtos;

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
        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        /// <response code="200">Returns a list of User objects.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User?>> getAllUsers()
        {
            var users = await _userService.GetUserAsync();
            return Ok(users);
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <response code="200">Returns the User object.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User?>> getUser(string id)
        {
            var users = await _userService.GetUserAsync(id);
            return Ok(users);
        }

        /// <summary>
        /// Gets simplified information for all users.
        /// </summary>
        /// <returns>A list of UserCreateDto objects containing simplified user information.</returns>
        /// <response code="200">Returns a list of UserCreateDto objects.</response>
        [HttpGet("GetUserInfo")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDtos.UserCreateDto?>> getAllUsersSmallData()
        {
            var users = await _userService.GetAllUserInfoAsync();
            return Ok(users);
        }

        [HttpGet("GetInterviewers")]
        [Authorize(Roles = "Admin, Recruiter, Interviewer, HR")]
        public async Task<IActionResult> getInterviewers()
        {
            try
            {
                var response = await _userService.GetInterviewers();
                return Ok(new
                {
                    success = true,
                    message = "All Interviewers are fetched successfuly",
                    data = response
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        /// <summary>
        /// Gets the user profile for a specific user ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user profile information.</returns>
        /// <response code="200">Returns the UserProfileDto object.</response>
        /// <response code="400">If the user ID is not provided.</response>
        /// <response code="404">If no user is found with the specified ID.</response>
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

        [HttpGet("GetUserProfileToUpdate/{id}")]
        [Authorize(Roles = "Admin, Reviewer, Recruiter, Interviewer, HR")]
        public async Task<IActionResult> getUserProfileToUpdate(string id)
        {
            try
            {
                var response = await _userService.GetUserProfileToUpdate(id);
                return Ok(response);
            }catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Gets the ID of the last created user.
        /// </summary>
        /// <returns>An object containing the last user ID.</returns>
        /// <response code="200">Returns an object with the user_id property.</response>
        [HttpGet("GetLastUserId")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetLastUserId()
        {
            var userid = await _userService.GetLastUserIdAsync();

            return Ok(new { user_id = userid });
        }

        [HttpPut("UpdatePassword")]
        [Authorize(Roles = "Admin, Reviewer, Recruiter, Interviewer, HR")]
        public async Task<IActionResult> updateUserPassword(UserDtos.UpdateUserPassword dto)
        {
            try
            {
                var response = await _userService.UpdateUserPassword(dto);
                return Ok(new { success = response, message = "User Password Updated" });
            }catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("UpdateProfile")]
        [Authorize(Roles = "Admin, Reviewer, Recruiter")]
        public async Task<IActionResult> updateUserProfile(UserDtos.UserDto dto)
        {
            try
            {
                var response = await _userService.UpdateUserProfile(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        //Create user from dashboard
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="dto">The user creation data.</param>
        /// <returns>A success message and the ID of the newly created user.</returns>
        /// <response code="200">Returns a success message and the user ID.</response>
        /// <response code="400">If there is an error during user creation.</response>
        [HttpPost("AddUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserDtos.UserCreateDto dto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(dto);
                return Ok(new { message = "User created", user_id = user.user_id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Update user form dahsboard
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="dto">The user update data.</param>
        /// <returns>A success message and the ID of the updated user.</returns>
        /// <response code="200">Returns a success message and the user ID.</response>
        /// <response code="400">If there is an error during user update.</response>
        [HttpPut("UpdateUser")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDtos.UserUpdateDto dto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(dto);
                return Ok(new { message = "User Updated", user_id = user.user_id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>A success message and the ID of the deleted user.</returns>
        /// <response code="200">Returns a success message and the user ID.</response>
        /// <response code="400">If there is an error during user deletion.</response>
        [HttpDelete("DeleteUser/{user_id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string user_id)
        {
            try
            {
                if (await _userService.DeleteUserAsync(user_id))
                    return Ok(new { message = "User Deleted", user_id = user_id });
                else return BadRequest("Error encounterd");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
