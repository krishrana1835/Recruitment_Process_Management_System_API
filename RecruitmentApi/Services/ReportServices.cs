using Microsoft.EntityFrameworkCore;
using MimeKit.Tnef;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using static RecruitmentApi.Dtos.ReportsDtos;

namespace RecruitmentApi.Services
{
    public class ReportServices
    {
        private readonly AppDbContext _context;

        public ReportServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportsDtos.InterviewSummaryResponse>> GetInterviewSummaryAsync(ReportsDtos.InterviewSummaryRequest request)
        {
            var interviewsQuery = _context.Interviews
                .Include(i => i.Candidate)
                .Include(i => i.Users)
                .Include(i => i.InterviewFeedbacks)
                .AsQueryable();

            interviewsQuery = interviewsQuery
                .Where(i => i.Users.Any(u => u.UserId == request.UserId));

            if (request.JobId.HasValue)
            {
                interviewsQuery = interviewsQuery
                    .Where(i => i.JobId == request.JobId.Value);
            }

            if (request.RoundNumber.HasValue)
            {
                interviewsQuery = interviewsQuery
                    .Where(i => i.RoundNumber == request.RoundNumber.Value);
            }

            if (request.InterviewDate.HasValue)
            {
                var date = request.InterviewDate.Value.Date;
                interviewsQuery = interviewsQuery
                    .Where(i => i.StartTime.Date == date);
            }

            var interviews = await interviewsQuery.ToListAsync();

            var groupedByJob = interviews.GroupBy(i => i.JobId);

            var response = groupedByJob.Select(group =>
            {
                var totalCandidates = group
                    .Select(i => i.CandidateId)
                    .Distinct()
                    .Count();

                var selectedInterviews = group
                    .Where(i => i.Status == "Selected");

                var allFeedbacks = selectedInterviews
                    .SelectMany(i => i.InterviewFeedbacks)
                    .Where(f => f.UserId == request.UserId);


                double averageScore = allFeedbacks.Any()
                    ? Math.Round(
                        allFeedbacks.Average(f =>
                            (f.ConceptRating + f.TechnicalRating) / 2.0
                        ), 2)
                    : 0;

                return new ReportsDtos.InterviewSummaryResponse
                {
                    JobId = group.Key,
                    TotalCandidates = totalCandidates,

                    SelectedCandidates = selectedInterviews
                        .Select(i => i.CandidateId)
                        .Distinct()
                        .Count(),

                    AverageScore = averageScore,

                    LastInterviewDate = group.Max(i => i.EndTime),

                    SelectedCandidateDetails = request.IncludeCandidateInfo
                        ? selectedInterviews
                            .GroupBy(i => i.CandidateId)
                            .Select(g => new ReportsDtos.CandidateSummaryDto
                            {
                                CandidateId = g.First().Candidate.CandidateId,
                                CandidateName = g.First().Candidate.FullName,
                                Email = g.First().Candidate.Email
                            })
                            .ToList()
                        : new List<ReportsDtos.CandidateSummaryDto>()
                };
            }).ToList();

            return response;
        }

        public async Task<List<ReportsDtos.CandidateDto>> GetTechWiseProfiles(ReportsDtos.TechReq request)
        {
            if (request.JobId <= 0)
                throw new ArgumentException("Invalid Job Id");

            var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == request.JobId);
            if (!jobExists)
                throw new Exception("Job not found");

            var shortlistedCandidateIds = await _context.Candidate_Status_Histories
                .Where(r => r.JobId == request.JobId)
                .GroupBy(r => new { r.CandidateId, r.JobId })
                .Select(g => new
                {
                    g.Key.CandidateId,
                    LatestChangedAt = g.Max(r => r.ChangedAt)
                })
                .Join(
                    _context.Candidate_Status_Histories,
                    latest => new { latest.CandidateId, ChangedAt = latest.LatestChangedAt },
                    history => new { history.CandidateId, history.ChangedAt },
                    (latest, history) => history
                )
                .Where(h => h.Status == "Shortlisted")
                .Select(h => h.CandidateId)
                .Distinct()
                .ToListAsync();

            var profiles = await _context.Candidates
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .Where(c =>
                    shortlistedCandidateIds.Contains(c.CandidateId) &&
                    request.SkillId.All(skillId =>
                        c.CandidateSkills.Any(cs => cs.SkillId == skillId)
                    )
                )
                .Select(c => new
                {
                    Candidate = c,
                    TotalExperience = c.CandidateSkills
                        .Where(cs => request.SkillId.Contains(cs.SkillId))
                        .Sum(cs => cs.YearsExperience)
                })
                .OrderByDescending(x => x.TotalExperience)
                .Select(x => new ReportsDtos.CandidateDto
                {
                    candidate_id = x.Candidate.CandidateId,
                    full_name = x.Candidate.FullName,
                    email = x.Candidate.Email,
                    totalExpirence = x.TotalExperience
                })
                .ToListAsync();

            return profiles;
        }

        public async Task<List<ReportsDtos.CandidateDto>> GetExpirienceWiseProfiles(ReportsDtos.ExpirienceReq request)
        {
            if (request.JobId <= 0)
                throw new ArgumentException("Invalid Job Id");

            var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == request.JobId);
            if (!jobExists)
                throw new Exception("Job not found");

            var shortlistedCandidateIds = await _context.Candidate_Status_Histories
                .Where(r => r.JobId == request.JobId)
                .GroupBy(r => new { r.CandidateId, r.JobId })
                .Select(g => new
                {
                    g.Key.CandidateId,
                    LatestChangedAt = g.Max(r => r.ChangedAt)
                })
                .Join(
                    _context.Candidate_Status_Histories,
                    latest => new { latest.CandidateId, ChangedAt = latest.LatestChangedAt },
                    history => new { history.CandidateId, history.ChangedAt },
                    (latest, history) => history
                )
                .Where(h => h.Status == "Shortlisted")
                .Select(h => h.CandidateId)
                .Distinct()
                .ToListAsync();

            var profiles = await _context.Candidates
                .Where(c =>
                    shortlistedCandidateIds.Contains(c.CandidateId) &&
                    request.SkillId.All(skillId =>
                        c.CandidateSkills.Any(cs => cs.SkillId == skillId)
                    )
                )
                .Select(c => new
                {
                    Candidate = c,

                    AverageExperience = c.CandidateSkills
                        .Where(cs => request.SkillId.Contains(cs.SkillId))
                        .Average(cs => (float?)cs.YearsExperience) ?? 0
                })
                .Where(x => x.AverageExperience >= request.MinExp)
                .OrderByDescending(x => x.AverageExperience)
                .Select(x => new ReportsDtos.CandidateDto
                {
                    candidate_id = x.Candidate.CandidateId,
                    full_name = x.Candidate.FullName,
                    email = x.Candidate.Email,
                    totalExpirence = x.AverageExperience
                })
                .ToListAsync();

            return profiles;
        }

        public async Task<ReportsDtos.DailySummaryDto> GetDailySummaryAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var summary = new ReportsDtos.DailySummaryDto
            {
                ReportDate = startDate
            };

            var candidatesQuery = _context.Candidates
                .Where(c => c.CreatedAt >= startDate && c.CreatedAt < endDate);

            summary.TotalCandidatesAdded = await candidatesQuery.CountAsync();

            summary.CandidatesAdded = await candidatesQuery
                .Select(c => new ReportsDtos.CandidateSummaryDto
                {
                    CandidateId = c.CandidateId,
                    CandidateName = c.FullName,
                    Email = c.Email
                })
                .ToListAsync();

            summary.EmailsSent = await _context.EmailMessages
                .Include(e => e.Recipients)
                .Where(e => e.IsSent &&
                            e.CreatedAt >= startDate &&
                            e.CreatedAt < endDate)
                .Select(e => new ReportsDtos.EmailSummaryDto
                {
                    MessageId = e.Id,
                    Subject = e.Subject,
                    SentAt = e.CreatedAt,
                    Recipients = e.Recipients.Select(r => r.Email).ToList()
                })
                .ToListAsync();

            var dateOnly = DateOnly.FromDateTime(startDate);

            summary.NewEmployees = await _context.Employee_Records
                .Include(e => e.Candidate)
                .Include(e => e.Job)
                .Where(e => e.JoiningDate == dateOnly)
                .Select(e => new ReportsDtos.EmployeeSummaryDto
                {
                    EmployeeId = e.EmployeeId,
                    CandidateId = e.CandidateId,
                    CandidateName = e.Candidate.FullName,
                    CandidateEmail = e.Candidate.Email,
                    JobId = e.JobId,
                    JobTitle = e.Job.JobTitle
                })
                .ToListAsync();

            var todaysInterviews = await _context.Interviews
                .Include(i => i.HrReviews)
                .Where(i => i.StartTime >= startDate &&
                            i.StartTime < endDate)
                .ToListAsync();

            summary.InterviewStats.TotalInterviewsTaken = todaysInterviews.Count;

            var hrInterviews = todaysInterviews
                .Where(i => i.HrReviews.Any())
                .ToList();

            var simpleInterviews = todaysInterviews
                .Where(i => !i.HrReviews.Any())
                .ToList();

            summary.InterviewStats.TotalHrInterviews = hrInterviews.Count;
            summary.InterviewStats.TotalSimpleInterviews = simpleInterviews.Count;

            summary.InterviewStats.SelectedInHr = hrInterviews.Count(i =>
                i.Status.Equals("Selected", StringComparison.OrdinalIgnoreCase));

            summary.InterviewStats.SelectedInSimple = simpleInterviews.Count(i =>
                i.Status.Equals("Selected", StringComparison.OrdinalIgnoreCase));

            summary.JobsCreated = await _context.Jobs
                .Where(j => j.CreatedAt >= startDate &&
                            j.CreatedAt < endDate)
                .Select(j => new ReportsDtos.JobSummaryDto
                {
                    JobId = j.JobId,
                    JobTitle = j.JobTitle,
                    Status = j.Status.Status
                })
                .ToListAsync();

            summary.JobsStatusChanged = await _context.Jobs_Statuses
                .Where(js => js.ChangedAt >= startDate &&
                             js.ChangedAt < endDate)
                .SelectMany(js => js.Jobs.Select(j => new ReportsDtos.JobSummaryDto
                {
                    JobId = j.JobId,
                    JobTitle = j.JobTitle,
                    Status = js.Status
                }))
                .ToListAsync();

            summary.UsersCreated = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.CreatedAt >= startDate &&
                            u.CreatedAt < endDate)
                .Select(u => new ReportsDtos.UserSummaryDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Roles = u.Roles.Select(r => new ReportsDtos.RoleDto
                    {
                        RoleName = r.RoleName
                    }).ToList()
                })
                .ToListAsync();

            return summary;
        }

        public async Task<ReportsDtos.CandidateSummaryReportDto?> GetCandidateSummaryAsync(string candidateId)
        {
            var summary = await _context.Candidates
                .Where(c => c.CandidateId == candidateId)
                .Select(c => new ReportsDtos.CandidateSummaryReportDto
                {
                    CandidateId = c.CandidateId,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    ResumePath = c.ResumePath,

                    Documents = c.CandidateDocuments.Select(d => new ReportsDtos.CandidateDocumentDto
                    {
                        DocumentId = d.DocumentId,
                        DocumentType = d.DocumentType,
                        FilePath = d.FilePath,
                        VerificationStatus = d.VerificationStatus,
                        UploadedAt = d.UploadedAt
                    }).ToList(),
                    
                    JobStatusHistories = c.CandidateStatusHistories
                        .GroupBy(h => new { h.Job.JobId, h.Job.JobTitle })
                        .Select(g => new ReportsDtos.JobStatusHistoryGroupDto
                        {
                            JobId = g.Key.JobId,
                            JobTitle = g.Key.JobTitle,
                            History = g.Select(h => new ReportsDtos.StatusHistoryDto
                            {
                                Status = h.Status,
                                Reason = h.Reason,
                                ChangedAt = h.ChangedAt,
                                ChangedBy = h.ChangedByUser.UserId
                            }).OrderByDescending(h => h.ChangedAt).ToList()
                        }).ToList(),

                    JobInterviews = c.Interviews
                        .GroupBy(i => new { i.Job.JobId, i.Job.JobTitle })
                        .Select(g => new ReportsDtos.JobInterviewSummaryDto
                        {
                            JobId = g.Key.JobId,
                            JobTitle = g.Key.JobTitle,

                            Rounds = g.GroupBy(r => new { r.RoundNumber, r.InterviewId })
                                .Select(rg => new ReportsDtos.InterviewRoundDto
                                {
                                    RoundNumber = rg.Key.RoundNumber,

                                    RoundTitle = rg.Select(i => i.InterviewType.InterviewRoundName).FirstOrDefault(),

                                    SkillFeedbacks = rg.SelectMany(i => i.InterviewFeedbacks)
                                        .Where(i => i.InterviewId == rg.Select(j=> j.InterviewId).FirstOrDefault())
                                        .Select(f => new ReportsDtos.SkillFeedbackDto
                                        {
                                            InterviewId = f.InterviewId,
                                            SkillName = f.CandidateSkill.Skill.SkillName,
                                            TechnicalRating = f.TechnicalRating,
                                            ConceptRating = f.ConceptRating,
                                            Comments = f.Comments,
                                            FeedbackByUserId = f.UserId
                                        }).ToList(),

                                    HrReviews = rg.SelectMany(i => i.HrReviews)
                                        .Select(hr => new ReportsDtos.HrReviewDto
                                        {
                                            InterviewId = hr.InterviewId,
                                            CommunicationRating = hr.CommunicationRating,
                                            OverallRating = hr.OverallRating,
                                            Strengths = hr.Strengths,
                                            AreasForImprovement = hr.AreasForImprovement,
                                            ReviewByUserId = hr.UserId,
                                            TeamworkRating = hr.TeamworkRating,
                                            AdaptabilityRating = hr.AdaptabilityRating,
                                            LeadershipRating= hr.LeadershipRating,
                                            TrainingRecommendations = hr.TrainingRecommendations,
                                            CareerPathNotes = hr.CareerPathNotes
                                        }).ToList()
                                })
                                .OrderBy(r => r.RoundNumber)
                                .ToList()
                        }).ToList(),

                    EmployeeRecord = c.EmployeeRecord == null ? null : new ReportsDtos.EmployeeRecordDto
                    {
                        EmployeeId = c.EmployeeRecord.EmployeeId,
                        JoiningDate = c.EmployeeRecord.JoiningDate,
                        OfferLetterPath = c.EmployeeRecord.OfferLetterPath,
                        JobId = c.EmployeeRecord.JobId,
                        JobTitle = c.EmployeeRecord.Job.JobTitle
                    }
                })
                .FirstOrDefaultAsync();

            return summary;
        }

    }
}
