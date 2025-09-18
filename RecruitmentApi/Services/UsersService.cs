using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using BCrypt.Net;
using RecruitmentApi.Data;
using RecruitmentApi.Models;
using RecruitmentApi.Dtos;

namespace RecruitmentApi.Services
{
    public class UsersService
    {
        private AppDbContext _context;
        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        //Get all the users information
        public async Task<List<User?>> GetUserAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        //Get infromation on specific user using id
        public async Task<User?> GetUserAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.user_id.Equals(id));
            return user;
        }

        //Get All users basic infromaation to display
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

        //Get user infromation of specific user for user profile
        public async Task<UserDtos.UserProfileDto?> GetUserProfileAsync(string id)
        {
            var data = await _context.Users.Include(r=> r.roles).Include(r => r.Candidate_Reviews).Include(r => r.Jobs).FirstOrDefaultAsync(r => r.user_id == id);

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
                jobs_created = data.Jobs.Select(r => new JobDtos.JobDto
                {
                    job_id = r.job_id,
                    job_title = r.job_title,
                    job_description = r.job_description,
                    created_at = r.created_at,
                    status_id = r.status_id,
                    status = new Jobs_StatusDtos.Jobs_StatusDto
                    {
                        status_id = r.status.status_id,
                        status = r.status.status,
                        reason =  r.status.reason,
                        changed_by = r.status.changed_by,
                        changed_at = r.status.changed_at
                    },
                }).ToList(),
            };
            return user;
        }

        //Add new user
        public async Task<UserDtos.UserDto?> AddUserAsync(User newUser)
        {
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var user = new UserDtos.UserDto
            {
                user_id = newUser.user_id,  
                name = newUser.name,
                email = newUser.email,
            };

            return user;
        }

        //public async Task<User?> UpdateUserAsync(User updateUser)
        //{

        //} 
    }
}
