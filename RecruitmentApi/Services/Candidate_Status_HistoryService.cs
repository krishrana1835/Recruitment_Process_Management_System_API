using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class Candidate_Status_HistoryService
    {
        private AppDbContext _context;

        public Candidate_Status_HistoryService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Represents the various statuses a candidate can have in the job application workflow.
        /// </summary>
        public enum CandidateStatus
        {
            Applied,                 // Candidate has submitted the application
            Shortlisted,             // Candidate has been shortlisted for next round
            InterviewScheduled,      // Interview has been scheduled
            InterviewCompleted,      // Candidate has completed the interview
            OnHold,                  // Application is temporarily on hold
            Accepted,
            Rejected,                // Candidate has been rejected
            Hired                    // Candidate has officially joined the company
        }

        private static bool IsValidCandidateStatus(string status)
        {
            return Enum.TryParse(typeof(CandidateStatus), status, true, out _);
        }

        public async Task<Boolean> CheckForApplicationAsync(Candidate_Status_HistoryDtos.JobApplicationByCandidate dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id);
            if (candidate == null)
                throw new Exception("Candidate not found");

            var job = await _context.Jobs.FirstOrDefaultAsync(r => r.job_id == dto.job_id);

            if (job == null)
                throw new Exception("Job not found");

            var job_status = await _context.Candidate_Status_Histories.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id && r.job_id == dto.job_id);

            if (job_status == null)
                return false;
            return true;
        }

        public async Task<List<Candidate_Status_HistoryDtos.JobApplicationStatus>?> GetJobapplicationStatus(string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == candidate_id);
            if (candidate == null)
                throw new Exception("Candidate not found");

            var status = await _context.Candidate_Status_Histories.Include(r => r.job).ThenInclude(j => j.status)
                .Where(r => r.candidate_id == candidate_id)
                .OrderByDescending(r => r.changed_at)
                .ToListAsync();

            if (status == null)
                return null;

            var response = status.Select(r => new Candidate_Status_HistoryDtos.JobApplicationStatus
            {
                candidate_status_id = r.candidate_status_id,
                status = r.status,
                changed_at = r.changed_at,
                job = new JobDtos.ListJobStatus
                {
                    job_title = r.job.job_title,
                    status = new Jobs_StatusDtos.ListJobStatus
                    {
                        status = r.job.status.status
                    }
                }
            }).ToList();

            return response;
        }

        public async Task<List<JobDtos.ListJobTitle>?> GetAppliedJobs(string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.AnyAsync(r => r.candidate_id == candidate_id);
            if (!candidate)
                throw new NullReferenceException("Candidate not found");

            var response = await _context.Candidate_Status_Histories
                             .Where(r => r.candidate_id == candidate_id)
                             .Select(r => new JobDtos.ListJobTitle
                             {
                                 job_id = r.job.job_id,
                                 job_title = r.job.job_title,
                                 sheduled = r.job.scheduled
                             })
                             .Distinct()
                             .ToListAsync();

            return response;
        }

        public async Task<Boolean> ApplyForJobAsync(Candidate_Status_HistoryDtos.JobApplicationByCandidate dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id);
            if (candidate == null)
                throw new Exception("Candidate not found");

            var job = await _context.Jobs.FirstOrDefaultAsync(r => r.job_id == dto.job_id);

            if (job == null)
                throw new Exception("Job not found");

            var changedBy = await _context.Users.Include(r => r.roles).Where(r => r.roles.Any(r => r.role_name == "Admin")).FirstOrDefaultAsync();

            if (changedBy == null)
                throw new Exception("Can not find user id to assign");

            var candidate_status = new Candidate_Status_History
            {
                status = CandidateStatus.Applied.ToString(),
                reason = "Application sent by candidate",
                changed_at = DateTime.Now,
                candidate_id = dto.candidate_id,
                job_id = dto.job_id,
                changed_by = changedBy.user_id
            };

            await _context.Candidate_Status_Histories.AddAsync(candidate_status);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Boolean> updateCandidateStatus(Candidate_Status_HistoryDtos.UpdateCandidateStatusRequest dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentNullException("Candidate id is not provided.");

            if(dto.changed_by == null) throw new ArgumentNullException("User Id is not provided");

            if (dto.status.IsNullOrEmpty())
                throw new ArgumentNullException("Status is not provided");

            if (!IsValidCandidateStatus(dto.status))
                throw new ArgumentException("Invalid status provided");

            if (dto.reason.IsNullOrEmpty())
                throw new ArgumentNullException("Reason is not provided");

            if (int.IsNegative(dto.job_id))
                throw new ArgumentException("Invaid job id");

            var candidate = await _context.Candidates.AnyAsync(c => c.candidate_id == dto.candidate_id);

            if (!candidate)
                throw new NullReferenceException("Candidate not found");

            var job = await _context.Jobs.AnyAsync(j => j.job_id == dto.job_id);

            if (!job)
                throw new NullReferenceException("Job not found");

            var user = await _context.Users.AnyAsync(c => c.user_id == dto.changed_by);

            if (!user)
                throw new NullReferenceException("User not found");

            var candidate_status = await _context.Candidate_Status_Histories.AnyAsync(r => r.candidate_id == dto.candidate_id && r.job_id == dto.job_id && r.status == CandidateStatus.Applied.ToString());

            if (!candidate_status)
                throw new NullReferenceException("Candidate Status not found");

            var status = new Candidate_Status_History
            {
                status = dto.status,
                reason = dto.reason,
                changed_at = DateTime.Now,
                candidate_id = dto.candidate_id,
                job_id = dto.job_id,
                changed_by = dto.changed_by,
            };

            await _context.Candidate_Status_Histories.AddAsync(status);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CandidateDtos.CandidateListDto>?> GetJobApplications(int job_id)
        {
            if (job_id <= 0)
                throw new ArgumentException("Invalid Job Id");

            var jobExists = await _context.Jobs.AnyAsync(r => r.job_id == job_id);
            if (!jobExists)
                throw new Exception("Job not found");
            
            var latestStatus = _context.Candidate_Status_Histories
                .Where(r => r.job_id == job_id)
                .GroupBy(r => r.candidate_id)
                .Select(g => new
                {
                    CandidateId = g.Key,
                    LatestChangedAt = g.Max(r => r.changed_at)
                });

            var appliedCandidates = await _context.Candidate_Status_Histories
                .Include(r => r.candidate)
                .Join(
                    latestStatus,
                    history => new { history.candidate_id, history.changed_at },
                    latest => new { candidate_id = latest.CandidateId, changed_at = latest.LatestChangedAt },
                    (history, latest) => history
                )
                .Where(r => r.status == CandidateStatus.Applied.ToString())
                .ToListAsync();

            if (!appliedCandidates.Any())
                return new List<CandidateDtos.CandidateListDto>();


            var response = appliedCandidates.Select(r => new CandidateDtos.CandidateListDto
            {
                candidate_id = r.candidate_id,
                full_name = r.candidate.full_name,
                email = r.candidate.email,
                phone = r.candidate.phone
            }).ToList();

            return response;
        }


    }
}
