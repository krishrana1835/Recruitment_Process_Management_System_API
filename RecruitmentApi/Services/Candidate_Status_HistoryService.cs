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
            Scheduled,      // Interview has been scheduled
            Rescheduled,      // Candidate has completed the interview
            Selected,
            Rejected,                // Candidate has been rejected
            Hired,                    // Candidate has officially joined the company
            Completed,
            Pending
        }

        private static bool IsValidCandidateStatus(string status)
        {
            return Enum.TryParse(typeof(CandidateStatus), status, true, out _);
        }

        public async Task<Boolean> CheckForApplicationAsync(int job_id, string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == candidate_id);
            if (candidate == null)
                throw new Exception("Candidate not found");

            var job = await _context.Jobs.FirstOrDefaultAsync(r => r.JobId == job_id);

            if (job == null)
                throw new Exception("Job not found");

            var job_status = await _context.Candidate_Status_Histories.FirstOrDefaultAsync(r => r.CandidateId == candidate_id && r.JobId == job_id);

            if (job_status == null)
                return false;
            return true;
        }

        public async Task<List<Candidate_Status_HistoryDtos.JobApplicationStatus>?> GetJobapplicationStatus(string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == candidate_id);
            if (candidate == null)
                throw new Exception("Candidate not found");

            var status = await _context.Candidate_Status_Histories.Include(r => r.Job).ThenInclude(j => j.Status)
                .Where(r => r.CandidateId == candidate_id)
                .OrderByDescending(r => r.ChangedAt)
                .ToListAsync();

            if (status == null)
                return null;

            var response = status.Select(r => new Candidate_Status_HistoryDtos.JobApplicationStatus
            {
                candidate_status_id = r.CandidateStatusId,
                status = r.Status,
                changed_at = r.ChangedAt,
                reason = r.Reason,
                job = new JobDtos.ListJobStatus
                {
                    job_title = r.Job.JobTitle,
                    status = new Jobs_StatusDtos.ListJobStatus
                    {
                        status = r.Job.Status.Status
                    }
                }
            }).ToList();

            return response;
        }

        public async Task<List<JobDtos.ListJobTitle>?> GetAppliedJobs(string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.AnyAsync(r => r.CandidateId == candidate_id);
            if (!candidate)
                throw new NullReferenceException("Candidate not found");

            var response = await _context.Candidate_Status_Histories
                             .Where(r => r.CandidateId == candidate_id)
                             .Select(r => new JobDtos.ListJobTitle
                             {
                                 job_id = r.Job.JobId,
                                 job_title = r.Job.JobTitle,
                                 scheduled = r.Job.Scheduled
                             })
                             .Distinct()
                             .ToListAsync();

            return response;
        }

        public async Task<Boolean> ApplyForJobAsync(Candidate_Status_HistoryDtos.JobApplicationByCandidate dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id);
            if (candidate == null)
                throw new Exception("Candidate not found");

            var job = await _context.Jobs.FirstOrDefaultAsync(r => r.JobId == dto.job_id);

            if (job == null)
                throw new Exception("Job not found");

            var changedBy = await _context.Users.Include(r => r.Roles).Where(r => r.Roles.Any(r => r.RoleName == "Admin")).FirstOrDefaultAsync();

            if (changedBy == null)
                throw new Exception("Can not find user id to assign");

            var candidate_status = new Candidate_Status_History
            {
                Status = CandidateStatus.Applied.ToString(),
                Reason = "Application sent by candidate",
                ChangedAt = DateTime.Now,
                CandidateId = dto.candidate_id,
                JobId = dto.job_id,
                ChangedBy = changedBy.UserId
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

            var candidate = await _context.Candidates.AnyAsync(c => c.CandidateId == dto.candidate_id);

            if (!candidate)
                throw new NullReferenceException("Candidate not found");

            var job = await _context.Jobs.AnyAsync(j => j.JobId == dto.job_id);

            if (!job)
                throw new NullReferenceException("Job not found");

            var user = await _context.Users.AnyAsync(c => c.UserId == dto.changed_by);

            if (!user)
                throw new NullReferenceException("User not found");

            var candidate_status = await _context.Candidate_Status_Histories.AnyAsync(r => r.CandidateId == dto.candidate_id && r.JobId == dto.job_id && r.Status == CandidateStatus.Applied.ToString());

            if (!candidate_status)
                throw new NullReferenceException("Candidate Status not found");

            var status = new Candidate_Status_History
            {
                Status = dto.status,
                Reason = dto.reason,
                ChangedAt = DateTime.Now,
                CandidateId = dto.candidate_id,
                JobId = dto.job_id,
                ChangedBy = dto.changed_by,
            };

            await _context.Candidate_Status_Histories.AddAsync(status);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CandidateDtos.CandidateListDto>?> GetJobApplications(int job_id)
        {
            if (job_id <= 0)
                throw new ArgumentException("Invalid Job Id");

            var jobExists = await _context.Jobs.AnyAsync(r => r.JobId == job_id);
            if (!jobExists)
                throw new Exception("Job not found");
            
            var latestStatus = _context.Candidate_Status_Histories
                .Where(r => r.JobId == job_id)
                .GroupBy(r => r.CandidateId)
                .Select(g => new
                {
                    CandidateId = g.Key,
                    LatestChangedAt = g.Max(r => r.ChangedAt)
                });

            var appliedCandidates = await _context.Candidate_Status_Histories
                .Include(r => r.Candidate)
                .Join(
                    latestStatus,
                    history => new { history.CandidateId, history.ChangedAt },
                    latest => new { CandidateId = latest.CandidateId, ChangedAt = latest.LatestChangedAt },
                    (history, latest) => history
                )
                .Where(r => r.Status == CandidateStatus.Applied.ToString())
                .ToListAsync();

            if (!appliedCandidates.Any())
                return new List<CandidateDtos.CandidateListDto>();


            var response = appliedCandidates.Select(r => new CandidateDtos.CandidateListDto
            {
                candidate_id = r.CandidateId,
                full_name = r.Candidate.FullName,
                email = r.Candidate.Email,
                phone = r.Candidate.Phone
            }).ToList();

            return response;
        }


    }
}
