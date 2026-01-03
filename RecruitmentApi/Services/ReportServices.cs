using RecruitmentApi.Data;
using Microsoft.EntityFrameworkCore;
using MimeKit.Tnef;
using RecruitmentApi.Dtos;

namespace RecruitmentApi.Services
{
    public class ReportServices
    {
        public class InterviewSummaryRequest
        {
            public string UserId { get; set; } = null!;
            public int? JobId { get; set; }
            public int? RoundNumber { get; set; }
            public bool IncludeCandidateInfo { get; set; } = true;
            public DateTime? InterviewDate { get; set; }
        }

        public class InterviewSummaryResponse
        {
            public int JobId { get; set; }
            public int TotalCandidates { get; set; }
            public int SelectedCandidates { get; set; }
            public double AverageScore { get; set; }
            public DateTime LastInterviewDate { get; set; }
            public List<CandidateSummaryDto> SelectedCandidateDetails { get; set; } = new();
        }

        public class CandidateSummaryDto
        {
            public string CandidateId { get; set; } = null!;
            public string CandidateName { get; set; } = null!;
            public string? Email { get; set; }
        }

        private readonly AppDbContext _context;

        public ReportServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InterviewSummaryResponse>> GetInterviewSummaryAsync(InterviewSummaryRequest request)
        {
            // STEP 1: Base query – ONLY interviews where user is in panel
            var interviewsQuery = _context.Interviews
                .Include(i => i.candidate)
                .Include(i => i.users)
                .Include(i => i.Interview_Feedbacks)
                .AsQueryable();

            // 🔒 Interview must have this user as panel member
            interviewsQuery = interviewsQuery
                .Where(i => i.users.Any(u => u.user_id == request.UserId));

            // ---------------- JOB FILTER ----------------
            if (request.JobId.HasValue)
            {
                interviewsQuery = interviewsQuery
                    .Where(i => i.job_id == request.JobId.Value);
            }

            // ---------------- ROUND FILTER ----------------
            if (request.RoundNumber.HasValue)
            {
                interviewsQuery = interviewsQuery
                    .Where(i => i.round_number == request.RoundNumber.Value);
            }

            // ---------------- DATE FILTER ----------------
            if (request.InterviewDate.HasValue)
            {
                var date = request.InterviewDate.Value.Date;
                interviewsQuery = interviewsQuery
                    .Where(i => i.start_time.Date == date);
            }

            var interviews = await interviewsQuery.ToListAsync();

            // STEP 2: Group by Job
            var groupedByJob = interviews.GroupBy(i => i.job_id);

            var response = groupedByJob.Select(group =>
            {
                var totalCandidates = group
                    .Select(i => i.candidate_id)
                    .Distinct()
                    .Count();

                var selectedInterviews = group
                    .Where(i => i.status == "Selected");

                var allFeedbacks = selectedInterviews
                    .SelectMany(i => i.Interview_Feedbacks)
                    .Where(f => f.user_id == request.UserId);


                double averageScore = allFeedbacks.Any()
                    ? Math.Round(
                        allFeedbacks.Average(f =>
                            (f.concept_rating + f.technical_rating) / 2.0
                        ), 2)
                    : 0;

                return new InterviewSummaryResponse
                {
                    JobId = group.Key,
                    TotalCandidates = totalCandidates,

                    SelectedCandidates = selectedInterviews
                        .Select(i => i.candidate_id)
                        .Distinct()
                        .Count(),

                    AverageScore = averageScore,

                    LastInterviewDate = group.Max(i => i.end_time),

                    SelectedCandidateDetails = request.IncludeCandidateInfo
                        ? selectedInterviews
                            .GroupBy(i => i.candidate_id)
                            .Select(g => new CandidateSummaryDto
                            {
                                CandidateId = g.First().candidate.candidate_id,
                                CandidateName = g.First().candidate.full_name,
                                Email = g.First().candidate.email
                            })
                            .ToList()
                        : new List<CandidateSummaryDto>()
                };
            }).ToList();

            return response;
        }

        public class TechReq
        {
            public int JobId { get; set; }
            public List<int> SkillId { get; set; } = new();

        }

        public class CandidateDto
        {
            public string candidate_id { get; set; } = null!;

            public string full_name { get; set; } = null!;

            public string email { get; set; } = null!;

            public float totalExpirence { get; set; }
        }

        public async Task<List<CandidateDto>> GetTechWiseProfiles(TechReq request)
        {
            if (request.JobId <= 0)
                throw new ArgumentException("Invalid Job Id");

            var jobExists = await _context.Jobs.AnyAsync(j => j.job_id == request.JobId);
            if (!jobExists)
                throw new Exception("Job not found");

            var shortlistedCandidateIds = await _context.Candidate_Status_Histories
                .Where(r => r.job_id == request.JobId)
                .GroupBy(r => new { r.candidate_id, r.job_id })
                .Select(g => new
                {
                    g.Key.candidate_id,
                    LatestChangedAt = g.Max(r => r.changed_at)
                })
                .Join(
                    _context.Candidate_Status_Histories,
                    latest => new { latest.candidate_id, changed_at = latest.LatestChangedAt },
                    history => new { history.candidate_id, history.changed_at },
                    (latest, history) => history
                )
                .Where(h => h.status == "Shortlisted")
                .Select(h => h.candidate_id)
                .Distinct()
                .ToListAsync();

            var profiles = await _context.Candidates
                .Include(c => c.Candidate_Skills)
                    .ThenInclude(cs => cs.skill)
                .Where(c =>
                    shortlistedCandidateIds.Contains(c.candidate_id) &&
                    request.SkillId.All(skillId =>
                        c.Candidate_Skills.Any(cs => cs.skill_id == skillId)
                    )
                )
                .Select(c => new
                {
                    Candidate = c,
                    TotalExperience = c.Candidate_Skills
                        .Where(cs => request.SkillId.Contains(cs.skill_id))
                        .Sum(cs => cs.years_experience)
                })
                .OrderByDescending(x => x.TotalExperience)
                .Select(x => new CandidateDto
                {
                    candidate_id = x.Candidate.candidate_id,
                    full_name = x.Candidate.full_name,
                    email = x.Candidate.email,
                    totalExpirence = x.TotalExperience
                })
                .ToListAsync();

            return profiles;
        }

        public class ExpirienceReq
        {
            public int JobId { get; set; }
            public List<int> SkillId { get; set; } = new();
            public int MinExp { get; set; } = 0;
        }

        public async Task<List<CandidateDto>> GetExpirienceWiseProfiles(ExpirienceReq request)
        {
            if (request.JobId <= 0)
                throw new ArgumentException("Invalid Job Id");

            var jobExists = await _context.Jobs.AnyAsync(j => j.job_id == request.JobId);
            if (!jobExists)
                throw new Exception("Job not found");

            var shortlistedCandidateIds = await _context.Candidate_Status_Histories
                .Where(r => r.job_id == request.JobId)
                .GroupBy(r => new { r.candidate_id, r.job_id })
                .Select(g => new
                {
                    g.Key.candidate_id,
                    LatestChangedAt = g.Max(r => r.changed_at)
                })
                .Join(
                    _context.Candidate_Status_Histories,
                    latest => new { latest.candidate_id, changed_at = latest.LatestChangedAt },
                    history => new { history.candidate_id, history.changed_at },
                    (latest, history) => history
                )
                .Where(h => h.status == "Shortlisted")
                .Select(h => h.candidate_id)
                .Distinct()
                .ToListAsync();

            var profiles = await _context.Candidates
                .Where(c =>
                    shortlistedCandidateIds.Contains(c.candidate_id) &&
                    request.SkillId.All(skillId =>
                        c.Candidate_Skills.Any(cs => cs.skill_id == skillId)
                    )
                )
                .Select(c => new
                {
                    Candidate = c,

                    AverageExperience = c.Candidate_Skills
                        .Where(cs => request.SkillId.Contains(cs.skill_id))
                        .Average(cs => (float?)cs.years_experience) ?? 0
                })
                .Where(x => x.AverageExperience >= request.MinExp)
                .OrderByDescending(x => x.AverageExperience)
                .Select(x => new CandidateDto
                {
                    candidate_id = x.Candidate.candidate_id,
                    full_name = x.Candidate.full_name,
                    email = x.Candidate.email,
                    totalExpirence = x.AverageExperience
                })
                .ToListAsync();

            return profiles;
        }
    }
}
