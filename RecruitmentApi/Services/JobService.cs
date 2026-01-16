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
                .Include(j => j.Status)
                .Select(s => new JobDtos.ListAllJobs
                {
                    job_id = s.JobId,
                    job_description = s.JobDescription,
                    job_title = s.JobTitle,
                    created_at  = s.CreatedAt,
                    status_id = s.StatusId,
                    scheduled = s.Scheduled,
                    status = new Jobs_StatusDtos.ListAllJobs
                    {
                        status_id = s.StatusId,
                        status = s.Status.Status,
                    }
                }).ToListAsync();

            return jobs;
        }

        public async Task<List<JobDtos.ListAllJobs>> GetScheduledJobs(string filter)
        {
            if(filter == "Scheduled")
            {
                var jobs = await _context.Jobs
                    .Include(j => j.Status)
                    .Where(j => j.Status.Status != "Closed" && j.Scheduled == "Scheduled")
                    .Select(s => new JobDtos.ListAllJobs
                    {
                        job_id = s.JobId,
                        job_description = s.JobDescription,
                        job_title = s.JobTitle,
                        created_at = s.CreatedAt,
                        status_id = s.StatusId,
                        scheduled = s.Scheduled,
                        status = new Jobs_StatusDtos.ListAllJobs
                        {
                            status_id = s.StatusId,
                            status = s.Status.Status,
                        }
                    }).ToListAsync();
                return jobs;
            }
            else if(filter == "All")
            {
                var jobs = await _context.Jobs
                    .Include(j => j.Status)
                    .Where(j => j.Status.Status != "Closed")
                    .Select(s => new JobDtos.ListAllJobs
                    {
                        job_id = s.JobId,
                        job_description = s.JobDescription,
                        job_title = s.JobTitle,
                        created_at = s.CreatedAt,
                        status_id = s.StatusId,
                        scheduled = s.Scheduled,
                        status = new Jobs_StatusDtos.ListAllJobs
                        {
                            status_id = s.StatusId,
                            status = s.Status.Status,
                        }
                    }).ToListAsync();
                return jobs;
            }
            else
            {
                throw new InvalidOperationException("Invalid filter");
            }
                
        }

        public async Task<List<JobDtos.ListJobTitle>> GetJobtitlesAsync(bool sorted)
        {
            var query = _context.Jobs
                .Where(j =>
                    j.Scheduled == "Scheduled" &&
                    j.Status != null &&
                    j.Status.Status != "Closed")
                .Select(r => new JobDtos.ListJobTitle
                {
                    job_id = r.JobId,
                    job_title = r.JobTitle,
                    scheduled = r.Scheduled
                });

            if (sorted)
            {
                query = query.OrderBy(j => j.job_title);
            }

            return await query.ToListAsync();
        }

        public async Task<JobDtos.UpdateJobDto> GetJobAsync(int job_id)
        {

            if (int.IsNegative(job_id))
                throw new ArgumentException("Invalid job id");

            var job = await _context.Jobs
                .Include(j => j.Status)
                .Include(j => j.JobsSkills)
                .FirstOrDefaultAsync(j => j.JobId == job_id);

            if (job == null)
                throw new Exception("Job not found");

            var jobStatusDto = new Jobs_StatusDtos.CreateJobStatusDto
            {
                status = job.Status.Status,
                reason = job.Status.Reason,
                changed_by = job.Status.ChangedBy
            };

            var jobSkillsDtos = job.JobsSkills.Select(skill => new Jobs_SkillsDtos.Jobs_SkillDto
            {
                job_id = skill.JobId,
                skill_id = skill.SkillId,
                skill_type = skill.SkillType
            }).ToList();

            var updateJobDto = new JobDtos.UpdateJobDto
            {
                job_id = job.JobId,
                job_title = job.JobTitle,
                job_description = job.JobDescription,
                created_by = job.CreatedBy,
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

            var user = await _context.Users.FirstOrDefaultAsync(r => r.UserId == dto.created_by) ?? throw new Exception("User not found");

            var jobStatus = new Jobs_Status
            {
                Status = dto.status.status,
                Reason = dto.status.reason,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = dto.status.changed_by
            };

            var jobEntity = new Job
            {
                JobTitle = dto.job_title,
                JobDescription = dto.job_description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.created_by,
                Status = jobStatus,
                Scheduled = "Pending"
            };

            _context.Jobs.Add(jobEntity);
            await _context.SaveChangesAsync(); 

            if (dto.Jobs_Skills != null && dto.Jobs_Skills.Any())
            {
                var jobSkills = dto.Jobs_Skills.Select(skillDto => new Jobs_Skill
                {
                    JobId = jobEntity.JobId,
                    SkillId = skillDto.skill_id,
                    SkillType = skillDto.skill_type == "Preferred" ? "P" : "R"
                }).ToList();

                await _context.Jobs_Skills.AddRangeAsync(jobSkills);
                await _context.SaveChangesAsync();
            }

            var status = await _context.Jobs_Statuses
                .Where(s => s.StatusId == jobEntity.StatusId)
                .Select(s => new Jobs_StatusDtos.CreateJobStatusDto
                {
                    status = s.Status,
                    reason = s.Reason,
                    changed_by = s.ChangedBy
                })
                .FirstOrDefaultAsync();

            var result = new JobDtos.CreateJobDto
            {
                job_title = jobEntity.JobTitle,
                job_description = jobEntity.JobDescription,
                created_by = jobEntity.CreatedBy,
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

            var user = await _context.Users.FirstOrDefaultAsync(r => r.UserId == dto.created_by);
            if (user == null)
                throw new Exception("User not found");

            var jobEntity = await _context.Jobs
                .Include(j => j.Status)
                .Include(j => j.JobsSkills)
                .FirstOrDefaultAsync(j => j.JobId == dto.job_id);

            if (jobEntity == null)
                throw new Exception("Job not found");

            jobEntity.JobTitle = dto.job_title;
            jobEntity.JobDescription = dto.job_description;
            jobEntity.CreatedBy = dto.created_by;

            if (jobEntity.Status == null)
            {
                jobEntity.Status = new Jobs_Status
                {
                    Status = dto.status.status,
                    Reason = dto.status.reason,
                    ChangedBy = dto.status.changed_by,
                    ChangedAt = DateTime.UtcNow
                };
            }
            else
            {
                jobEntity.Status.Status = dto.status.status;
                jobEntity.Status.Reason = dto.status.reason;
                jobEntity.Status.ChangedBy = dto.status.changed_by;
                jobEntity.Status.ChangedAt = DateTime.UtcNow;
            }

            var existingSkills = _context.Jobs_Skills.Where(js => js.JobId == jobEntity.JobId);
            _context.Jobs_Skills.RemoveRange(existingSkills);

            if (dto.Jobs_Skills != null && dto.Jobs_Skills.Any())
            {
                var jobSkills = dto.Jobs_Skills.Select(skillDto => new Jobs_Skill
                {
                    JobId = jobEntity.JobId,
                    SkillId = skillDto.skill_id,
                    SkillType = skillDto.skill_type == "Preferred" ? "P" : "R"
                }).ToList();

                await _context.Jobs_Skills.AddRangeAsync(jobSkills);
            }

            await _context.SaveChangesAsync();

            var updatedStatus = await _context.Jobs_Statuses
                .Where(s => s.StatusId == jobEntity.StatusId)
                .Select(s => new Jobs_StatusDtos.CreateJobStatusDto
                {
                    status = s.Status,
                    reason = s.Reason,
                    changed_by = s.ChangedBy
                })
                .FirstOrDefaultAsync();

            var result = new JobDtos.UpdateJobDto
            {
                job_id = jobEntity.JobId,
                job_title = jobEntity.JobTitle,
                job_description = jobEntity.JobDescription,
                created_by = jobEntity.CreatedBy,
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
                .Include(j => j.Status)
                .Include(j => j.JobsSkills)
                .FirstOrDefaultAsync(j => j.JobId == job_id);

            if (job == null)
                throw new Exception("Job not found");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
