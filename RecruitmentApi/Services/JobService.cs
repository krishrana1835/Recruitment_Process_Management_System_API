using System.Security.AccessControl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class JobService
    {
        private AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public JobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobDtos.ListAllJobs>> GetAllJobsAsync()
        {
            var jobs = await _context.Jobs
                .Include(j => j.status)
                .Select(s => new JobDtos.ListAllJobs
                {
                    job_id = s.job_id,
                    job_description = s.job_description,
                    job_title = s.job_title,
                    created_at  = s.created_at,
                    status_id = s.status_id,
                    status = new Jobs_StatusDtos.ListAllJobs
                    {
                        status_id = s.status_id,
                        status = s.status.status,
                    }
                }).ToListAsync();

            return jobs;
        }

        public async Task<JobDtos.UpdateJobDto> GetJobAsync(int job_id)
        {

            if (int.IsNegative(job_id))
                throw new ArgumentException("Invalid job id");

            var job = await _context.Jobs
                .Include(j => j.status)
                .Include(j => j.Jobs_Skills)
                .FirstOrDefaultAsync(j => j.job_id == job_id);

            if (job == null)
                throw new Exception("Job not found");

            var jobStatusDto = new Jobs_StatusDtos.CreateJobStatusDto
            {
                status = job.status.status,
                reason = job.status.reason,
                changed_by = job.status.changed_by
            };

            var jobSkillsDtos = job.Jobs_Skills.Select(skill => new Jobs_SkillsDtos.AddJobs_SkillDto
            {
                job_id = skill.job_id,
                skill_ids = skill.skill_id,
                skill_type = skill.skill_type
            }).ToList();

            var updateJobDto = new JobDtos.UpdateJobDto
            {
                job_id = job.job_id,
                job_title = job.job_title,
                job_description = job.job_description,
                created_by = job.created_by,
                status = jobStatusDto,
                Jobs_Skills = jobSkillsDtos
            };

            return updateJobDto;
        }

        public async Task<JobDtos.CreateJobDto> AddJobAsync(JobDtos.CreateJobDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.job_title))
                throw new ArgumentException("Job title is required.");

            if (string.IsNullOrWhiteSpace(dto.job_description))
                throw new ArgumentException("Job description is required.");

            if (dto.status == null || string.IsNullOrWhiteSpace(dto.status.status))
                throw new ArgumentException("Status is required.");

            if (string.IsNullOrWhiteSpace(dto.status.reason))
                throw new ArgumentException("Reason is required.");

            if (string.IsNullOrWhiteSpace(dto.status.changed_by))
                throw new ArgumentException("Changed By field is required.");

            if (string.IsNullOrWhiteSpace(dto.created_by))
                throw new ArgumentException("Created By field is required.");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.user_id == dto.created_by);
            if (user == null)
                throw new Exception("User not found");

            var jobStatus = new Jobs_Status
            {
                status = dto.status.status,
                reason = dto.status.reason,
                changed_at = DateTime.UtcNow,
                changed_by = dto.status.changed_by
            };

            var jobEntity = new Job
            {
                job_title = dto.job_title,
                job_description = dto.job_description,
                created_at = DateTime.UtcNow,
                created_by = dto.created_by,
                status = jobStatus
            };

            _context.Jobs.Add(jobEntity);
            await _context.SaveChangesAsync(); 

            if (dto.Jobs_Skills != null && dto.Jobs_Skills.Any())
            {
                var jobSkills = dto.Jobs_Skills.Select(skillDto => new Jobs_Skill
                {
                    job_id = jobEntity.job_id,
                    skill_id = skillDto.skill_ids,
                    skill_type = skillDto.skill_type == "Preferred" ? "P" : "R"
                }).ToList();

                await _context.Jobs_Skills.AddRangeAsync(jobSkills);
                await _context.SaveChangesAsync();
            }

            var status = await _context.Jobs_Statuses
                .Where(s => s.status_id == jobEntity.status_id)
                .Select(s => new Jobs_StatusDtos.CreateJobStatusDto
                {
                    status = s.status,
                    reason = s.reason,
                    changed_by = s.changed_by
                })
                .FirstOrDefaultAsync();

            var result = new JobDtos.CreateJobDto
            {
                job_title = jobEntity.job_title,
                job_description = jobEntity.job_description,
                created_by = jobEntity.created_by,
                status = status ?? new Jobs_StatusDtos.CreateJobStatusDto(),
                Jobs_Skills = dto.Jobs_Skills
            };

            return result;
        }

        public async Task<JobDtos.UpdateJobDto> UpdateJobAsync(JobDtos.UpdateJobDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.job_title))
                throw new ArgumentException("Job title is required.");

            if (string.IsNullOrWhiteSpace(dto.job_description))
                throw new ArgumentException("Job description is required.");

            if (dto.status == null || string.IsNullOrWhiteSpace(dto.status.status))
                throw new ArgumentException("Status is required.");

            if (string.IsNullOrWhiteSpace(dto.status.reason))
                throw new ArgumentException("Reason is required.");

            if (string.IsNullOrWhiteSpace(dto.status.changed_by))
                throw new ArgumentException("Changed By field is required.");

            if (string.IsNullOrWhiteSpace(dto.created_by))
                throw new ArgumentException("Created By field is required.");

            var user = await _context.Users.FirstOrDefaultAsync(r => r.user_id == dto.created_by);
            if (user == null)
                throw new Exception("User not found");

            var jobEntity = await _context.Jobs
                .Include(j => j.status)
                .Include(j => j.Jobs_Skills)
                .FirstOrDefaultAsync(j => j.job_id == dto.job_id);

            if (jobEntity == null)
                throw new Exception("Job not found");

            jobEntity.job_title = dto.job_title;
            jobEntity.job_description = dto.job_description;
            jobEntity.created_by = dto.created_by;

            if (jobEntity.status == null)
            {
                jobEntity.status = new Jobs_Status
                {
                    status = dto.status.status,
                    reason = dto.status.reason,
                    changed_by = dto.status.changed_by,
                    changed_at = DateTime.UtcNow
                };
            }
            else
            {
                jobEntity.status.status = dto.status.status;
                jobEntity.status.reason = dto.status.reason;
                jobEntity.status.changed_by = dto.status.changed_by;
                jobEntity.status.changed_at = DateTime.UtcNow;
            }

            var existingSkills = _context.Jobs_Skills.Where(js => js.job_id == jobEntity.job_id);
            _context.Jobs_Skills.RemoveRange(existingSkills);

            if (dto.Jobs_Skills != null && dto.Jobs_Skills.Any())
            {
                var jobSkills = dto.Jobs_Skills.Select(skillDto => new Jobs_Skill
                {
                    job_id = jobEntity.job_id,
                    skill_id = skillDto.skill_ids,
                    skill_type = skillDto.skill_type == "Preferred" ? "P" : "R"
                }).ToList();

                await _context.Jobs_Skills.AddRangeAsync(jobSkills);
            }

            await _context.SaveChangesAsync();

            var updatedStatus = await _context.Jobs_Statuses
                .Where(s => s.status_id == jobEntity.status_id)
                .Select(s => new Jobs_StatusDtos.CreateJobStatusDto
                {
                    status = s.status,
                    reason = s.reason,
                    changed_by = s.changed_by
                })
                .FirstOrDefaultAsync();

            var result = new JobDtos.UpdateJobDto
            {
                job_id = jobEntity.job_id,
                job_title = jobEntity.job_title,
                job_description = jobEntity.job_description,
                created_by = jobEntity.created_by,
                status = updatedStatus ?? new Jobs_StatusDtos.CreateJobStatusDto(),
                Jobs_Skills = dto.Jobs_Skills
            };

            return result;
        }

        public async Task<bool> DeleteJobAsync(int job_id)
        {
            if (job_id <= 0)
                throw new ArgumentException("Invalid job id");

            var job = await _context.Jobs
                .Include(j => j.status)
                .Include(j => j.Jobs_Skills)
                .FirstOrDefaultAsync(j => j.job_id == job_id);

            if (job == null)
                throw new Exception("Job not found");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
