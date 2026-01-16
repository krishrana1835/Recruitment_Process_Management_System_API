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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId.Equals(id));
            return user;
        }

        /// <summary>
        /// Gets the ID of the last created user.
        /// </summary>
        /// <returns>The user ID of the last created user, or null if no users exist.</returns>
        public async Task<String?> GetLastUserIdAsync()
        {
            var user = await _context.Users.OrderByDescending(u => u.CreatedAt).FirstOrDefaultAsync();
            return user?.UserId;
        }

        /// <summary>
        /// Gets basic information for all users.
        /// </summary>
        /// <returns>A list of <see cref="UserDtos.UserInfoDto"/> objects containing basic user information, or null if no users exist.</returns>
        public async Task<List<UserDtos.UserInfoDto>?> GetAllUserInfoAsync()
        {
            var users = await _context.Users.Include(x => x.Roles).ToListAsync();

            if (users == null)
                return null;

            var data = users.Select(user => new UserDtos.UserInfoDto
            {
                user_id = user.UserId,
                name = user.Name,
                email = user.Email,
                roles = user.Roles.Select(r => new RoleDtos.RoleDto
                {
                    role_id = r.RoleId,
                    role_name = r.RoleName
                }).ToList(),
                created_at = user.CreatedAt
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
    .Include(r => r.Roles)
    .Include(r => r.CandidateReviews)
    .Include(r => r.InterviewFeedbacks)
    .Include(r => r.Jobs)
        .ThenInclude(j => j.Status) // 👈 include the status of each job
    .FirstOrDefaultAsync(r => r.UserId == id);


            if (data == null) return null;

            var user = new UserDtos.UserProfileDto
            {
                user_id = data.UserId,
                name = data.Name,
                email = data.Email,

                roles = data.Roles.Select(r => new RoleDtos.RoleDto
                {
                    role_id = r.RoleId,
                    role_name = r.RoleName
                }).ToList(),

                created_at = data.CreatedAt,

                interview_feedbacks = data.InterviewFeedbacks.Select(r => new Interview_FeedBackDtos.Interview_FeedbackDto
                {
                    feedback_id = r.FeedbackId,
                    rating = r.ConceptRating,
                    comments = r.Comments,
                    feedback_at = r.FeedbackAt,
                    interview_id = r.InterviewId,
                    candidate_skill_id = r.CandidateSkillId,
                }).ToList(),

                candidate_reviews = data.CandidateReviews.Select(r => new Candidate_ReviewDtos.Candidate_ReviewDto
                {
                    review_id = r.ReviewId,
                    comments = r.Comments,
                    reviewed_at = r.ReviewedAt,
                    candidate_id = r.CandidateId,
                    job_id = r.JobId,
                }).ToList(),

                jobs_created = data.Jobs?.Select(r => new JobDtos.JobDto
                {
                    job_id = r.JobId,
                    job_title = r.JobTitle,
                    job_description = r.JobDescription,
                    created_at = r.CreatedAt,
                    status_id = r.StatusId,
                    status = r.Status == null ? null : new Jobs_StatusDtos.Jobs_StatusDto
                    {
                        status_id = r.Status.StatusId,
                        status = r.Status.Status,
                        reason = r.Status.Reason,
                        changed_by = r.Status.ChangedBy,
                        changed_at = r.Status.ChangedAt
                    }
                }).ToList() ?? new List<JobDtos.JobDto>(),
            };

            return user;
        }

        public async Task<List<UserDtos.InterviewerInfo>> GetInterviewers()
        {
            var data = await _context.Users.Where(r => r.Roles.Any(r => r.RoleName == "Interviewer" || r.RoleName == "HR")).Select(r => new UserDtos.InterviewerInfo
            {
                user_id = r.UserId,
                name = r.Name,
                roles = r.Roles.Select(s => new RoleDtos.RoleDto
                {
                    role_name = s.RoleName,
                }).ToList()
            }).ToListAsync();
            if (data == null)
                return [];

            return data;
        } 

        public async Task<UserDtos.UserDto> GetUserProfileToUpdate(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentException("Invalid user id");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.UserId == id);

            if (user == null)
                throw new Exception("User not found");

            var response = new UserDtos.UserDto
            {
                user_id = user.UserId,
                name = user.Name,
                email = user.Email
            };

            return response;
        }

        public async Task<Boolean> UpdateUserPassword(UserDtos.UpdateUserPassword dto)
        {
            if (dto.user_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid user id");

            if (dto.password.IsNullOrEmpty())
                throw new ArgumentException("Password field is empty");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.UserId == dto.user_id);

            if (user == null)
                throw new Exception("User not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            user.PasswordHash = hashedPassword;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserDtos.UserDto> UpdateUserProfile(UserDtos.UserDto dto)
        {
            if (dto.user_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate id");

            if (dto.name.IsNullOrEmpty() || dto.email.IsNullOrEmpty())
                throw new ArgumentException("One or more fields are empty");

            var checkmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.email);

            if (checkmail != null)
                throw new Exception("Email already in use");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.UserId == dto.user_id);

            if (user == null)
                throw new Exception("User not found");

            user.Name = dto.name;
            user.Email = dto.email;

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
            if (await _context.Users.AnyAsync(u => u.UserId == dto.user_id))
                throw new Exception("User ID already exists");

            // Hash password (basic example, replace with real hasher)
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            var user = new User
            {
                UserId = dto.user_id,
                Name = dto.name,
                Email = dto.email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.Now,
            };

            // Attach existing roles by ID (ignore names)
            foreach (var roleDto in dto.roles)
            {
                var role = await _context.Roles.FindAsync(roleDto.role_id);
                if (role != null)
                {
                    user.Roles.Add(role);
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
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == dto.user_id);

            if (existingUser == null)
                throw new Exception("User ID does not exist");

            existingUser.Name = dto.name;
            existingUser.Email = dto.email;

            existingUser.Roles.Clear();

            foreach (var roleDto in dto.roles)
            {
                var role = await _context.Roles.FindAsync(roleDto.role_id);
                if (role != null)
                {
                    existingUser.Roles.Add(role);
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
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                throw new Exception("User Does not exist");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}