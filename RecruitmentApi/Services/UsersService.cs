using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using BCrypt.Net;
using RecruitmentApi.Data;
using RecruitmentApi.Models;
using RecruitmentApi.Dtos;
using static RecruitmentApi.Dtos.UserDtos;
using Microsoft.IdentityModel.Tokens;

namespace RecruitmentApi.Services
{
    /// <summary>
    /// Service for managing user-related operations.
    /// </summary>
    public class UsersService
    {
        private AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of all <see cref="User"/> objects.</returns>
        public async Task<List<User?>> GetUserAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        /// <summary>
        /// Gets information for a specific user by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The <see cref="User"/> object for the specified ID, or null if not found.</returns>
        public async Task<User?> GetUserAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.user_id.Equals(id));
            return user;
        }

        /// <summary>
        /// Gets the ID of the last created user.
        /// </summary>
        /// <returns>The user ID of the last created user, or null if no users exist.</returns>
        public async Task<String?> GetLastUserIdAsync()
        {
            var user = await _context.Users.OrderByDescending(u => u.created_at).FirstOrDefaultAsync();
            return user?.user_id;
        }

        /// <summary>
        /// Gets basic information for all users.
        /// </summary>
        /// <returns>A list of <see cref="UserDtos.UserInfoDto"/> objects containing basic user information, or null if no users exist.</returns>
        public async Task<List<UserDtos.UserInfoDto>?> GetAllUserInfoAsync()
        {
            var users = await _context.Users.Include(x => x.roles).ToListAsync();

            if (users == null)
                return null;

            var data = users.Select(user => new UserDtos.UserInfoDto
            {
                user_id = user.user_id,
                name = user.name,
                email = user.email,
                roles = user.roles.Select(r => new RoleDtos.RoleDto
                {
                    role_id = r.role_id,
                    role_name = r.role_name
                }).ToList(),
                created_at = user.created_at
            }).ToList();

            return data;
        }

        /// <summary>
        /// Gets detailed user profile information for a specific user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>A <see cref="UserDtos.UserProfileDto"/> object containing detailed user profile information, or null if the user is not found.</returns>
        public async Task<UserDtos.UserProfileDto?> GetUserProfileAsync(string id)
        {
            var data = await _context.Users
    .Include(r => r.roles)
    .Include(r => r.Candidate_Reviews)
    .Include(r => r.Interview_Feedbacks)
    .Include(r => r.Jobs)
        .ThenInclude(j => j.status) // 👈 include the status of each job
    .FirstOrDefaultAsync(r => r.user_id == id);


            if (data == null) return null;

            var user = new UserDtos.UserProfileDto
            {
                user_id = data.user_id,
                name = data.name,
                email = data.email,

                roles = data.roles.Select(r => new RoleDtos.RoleDto
                {
                    role_id = r.role_id,
                    role_name = r.role_name
                }).ToList(),

                created_at = data.created_at,

                interview_feedbacks = data.Interview_Feedbacks.Select(r => new Interview_FeedBackDtos.Interview_FeedbackDto
                {
                    feedback_id = r.feedback_id,
                    rating = r.rating,
                    comments = r.comments,
                    feedback_at = r.feedback_at,
                    interview_id = r.interview_id,
                    skill_id = r.skill_id,
                }).ToList(),

                candidate_reviews = data.Candidate_Reviews.Select(r => new Candidate_ReviewDtos.Candidate_ReviewDto
                {
                    review_id = r.review_id,
                    comments = r.comments,
                    reviewed_at = r.reviewed_at,
                    candidate_id = r.candidate_id,
                    job_id = r.job_id,
                }).ToList(),

                jobs_created = data.Jobs?.Select(r => new JobDtos.JobDto
                {
                    job_id = r.job_id,
                    job_title = r.job_title,
                    job_description = r.job_description,
                    created_at = r.created_at,
                    status_id = r.status_id,
                    status = r.status == null ? null : new Jobs_StatusDtos.Jobs_StatusDto
                    {
                        status_id = r.status.status_id,
                        status = r.status.status,
                        reason = r.status.reason,
                        changed_by = r.status.changed_by,
                        changed_at = r.status.changed_at
                    }
                }).ToList() ?? new List<JobDtos.JobDto>(),
            };

            return user;
        }

        public async Task<UserDtos.UserDto> GetUserProfileToUpdate(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentException("Invalid user id");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.user_id == id);

            if (user == null)
                throw new Exception("User not found");

            var response = new UserDtos.UserDto
            {
                user_id = user.user_id,
                name = user.name,
                email = user.email
            };

            return response;
        }

        public async Task<Boolean> UpdateUserPassword(UserDtos.UpdateUserPassword dto)
        {
            if (dto.user_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid user id");

            if (dto.password.IsNullOrEmpty())
                throw new ArgumentException("Password field is empty");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.user_id == dto.user_id);

            if (user == null)
                throw new Exception("User not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            user.password_hash = hashedPassword;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserDtos.UserDto> UpdateUserProfile(UserDtos.UserDto dto)
        {
            if (dto.user_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate id");

            if (dto.name.IsNullOrEmpty() || dto.email.IsNullOrEmpty())
                throw new ArgumentException("One or more fields are empty");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.user_id == dto.user_id);

            if (user == null)
                throw new Exception("User not found");

            user.name = dto.name;
            user.email = dto.email;

            await _context.SaveChangesAsync();

            return dto;

        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="dto">The user creation data.</param>
        /// <returns>The newly created <see cref="User"/> object.</returns>
        /// <exception cref="Exception">Thrown if the user ID already exists.</exception>
        public async Task<User> CreateUserAsync(UserDtos.UserCreateDto dto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.user_id == dto.user_id))
                throw new Exception("User ID already exists");

            // Hash password (basic example, replace with real hasher)
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            var user = new User
            {
                user_id = dto.user_id,
                name = dto.name,
                email = dto.email,
                password_hash = hashedPassword,
                created_at = DateTime.Now,
            };

            // Attach existing roles by ID (ignore names)
            foreach (var roleDto in dto.roles)
            {
                var role = await _context.Roles.FindAsync(roleDto.role_id);
                if (role != null)
                {
                    user.roles.Add(role);
                }
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }


        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="dto">The user update data.</param>
        /// <returns>The updated <see cref="User"/> object.</returns>
        /// <exception cref="Exception">Thrown if the user ID does not exist.</exception>
        public async Task<User> UpdateUserAsync(UserDtos.UserUpdateDto dto)
        {
            var existingUser = await _context.Users
                .Include(u => u.roles)
                .FirstOrDefaultAsync(u => u.user_id == dto.user_id);

            if (existingUser == null)
                throw new Exception("User ID does not exist");

            existingUser.name = dto.name;
            existingUser.email = dto.email;

            existingUser.roles.Clear();

            foreach (var roleDto in dto.roles)
            {
                var role = await _context.Roles.FindAsync(roleDto.role_id);
                if (role != null)
                {
                    existingUser.roles.Add(role);
                }
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>True if the user was successfully deleted; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown if the user does not exist.</exception>
        public async Task<bool> DeleteUserAsync(string userId)
        {
            if (userId == null)
                throw new Exception("User Id can not be null value");
            var user = await _context.Users
                .Include(u => u.roles)
                .FirstOrDefaultAsync(u => u.user_id == userId);
            if (user == null)
                throw new Exception("User Does not exist");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}