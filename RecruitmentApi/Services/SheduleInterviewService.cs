using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using NuGet.DependencyResolver;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class SheduleInterviewService
    {
        private readonly AppDbContext _context;

        public SheduleInterviewService(AppDbContext context)
        {
            _context = context;
        }

        public enum Access
        {
            Interviewer,
            HR
        }

            public class ScheduleInterviewRequestDto
            {
                public int job_id { get; set; }
                public int round_number { get; set; }

                public Access accessTo { get; set; }
                public DateTime scheduled_start_time { get; set; }
                public int duration_per_interview { get; set; }

                public List<List<string>> interviewers { get; set; } = new();

                public List<string>? panel_links { get; set; } = new();

                public string scheduled_by { get; set; } = string.Empty;

                public string? location { get; set; }

                public int interview_type_id { get; set; }
                public int result_of { get; set; }
            }

        public async Task<List<string>> GetShortlistedCandidates(int job_id)
        {
            if (job_id <= 0)
                throw new ArgumentException("Invalid Job Id");

            var jobExists = await _context.Jobs.AnyAsync(r => r.JobId == job_id);
            if (!jobExists)
                throw new Exception("Job not found");

            var latestStatus = _context.Candidate_Status_Histories
                .Where(r => r.JobId == job_id)
                .GroupBy(r => new { r.CandidateId, r.JobId })
                .Select(g => new
                {
                    g.Key.CandidateId,
                    g.Key.JobId,
                    LatestChangedAt = g.Max(r => r.ChangedAt)
                });

            var shortlisted = await _context.Candidate_Status_Histories
                .Join(
                    latestStatus,
                    history => new { history.CandidateId, history.JobId, history.ChangedAt },
                    latest => new { latest.CandidateId, latest.JobId, ChangedAt = latest.LatestChangedAt },
                    (history, latest) => history
                )
                .Where(r => r.Status == "Shortlisted")
                .Select(r => r.CandidateId)
                .ToListAsync();

            if (!shortlisted.Any())
                throw new Exception("No shortlisted candidates found for this job");

            return shortlisted;
        }

        public async Task<List<CandidateDtos.UpdateCandidateDto>> GetAllShortlistedCandidates(int job_id)
        {
            if (!await _context.Jobs.AnyAsync(j => j.JobId == job_id))
                throw new NullReferenceException("Job does not exist");
            var candidates = await GetShortlistedCandidates(job_id);

            List<CandidateDtos.UpdateCandidateDto> clist = [];
            foreach(var c in candidates)
            {
                var candidate = await _context.Candidates.FirstOrDefaultAsync(i => i.CandidateId == c) ?? throw new NullReferenceException("Candidate does not exist");
                clist.Add(new CandidateDtos.UpdateCandidateDto
                {
                    candidate_id = candidate.CandidateId,
                    full_name = candidate.FullName,
                    email = candidate.Email,
                    phone = candidate.Phone,
                    resume_path = candidate.ResumePath,
                });
            }

            return clist;
        }

        public async Task<Boolean> ScheduleInterviews(ScheduleInterviewRequestDto request)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == request.job_id) ?? throw new NullReferenceException("Job Does not exist");

            //var interview = await _context.Interviews.AnyAsync(i => i.JobId == request.job_id && i.RoundNumber == request.round_number);
            //if (interview)
            //    throw new Exception("Interview Alerady Exist");

            var interviewers_sorted = request.interviewers
                .OrderBy(panel => panel.Last().Equals("Offline", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ToList();

            if (request.accessTo != Access.Interviewer && request.accessTo != Access.HR)
                throw new InvalidDataException("Invalid AccessTo data");

            List<string> candidates = [];
            if(request.result_of == 0)
            {
                candidates = await GetShortlistedCandidates(request.job_id)
                             ?? throw new NullReferenceException("No candidates are shortlisted");
            }
            else
            {
                candidates = await _context.Interviews.Where(i => i.JobId == request.job_id && i.RoundNumber == request.result_of && i.Status == "Selected").Select(u => u.CandidateId).ToListAsync();
            }

            Console.WriteLine($"Candidates found: {string.Join(",", candidates)}");

            int panels = interviewers_sorted.Count;
            int total_candidates = candidates.Count;
            int total_slots = (int)Math.Ceiling(total_candidates / (double)panels);

            int candidate_index = 0;

            if (request.panel_links == null)
                request.panel_links = new List<string>();

            for (int slot = 0; slot < total_slots; slot++)
            {
                var start_time = request.scheduled_start_time.AddMinutes(slot * request.duration_per_interview);
                var end_time = start_time.AddMinutes(request.duration_per_interview);

                for (int panel_id = 0; panel_id < panels; panel_id++)
                {
                    if (candidate_index >= total_candidates)
                        break;

                    string candidate_id = candidates[candidate_index];
                    var panel = interviewers_sorted[panel_id];
                    var panel_members = panel.Take(panel.Count - 1).ToList();
                    string mode = panel.Last();

                    string locationOrLink;

                    if (mode.Equals("Offline", StringComparison.OrdinalIgnoreCase))
                    {
                        locationOrLink = request.location ?? "TBD";
                    }
                    else
                    {
                        if (request.panel_links.Count <= panel_id)
                            throw new Exception($"Missing link for online panel #{panel_id + 1}");

                        locationOrLink = request.panel_links[panel_id];
                    }

                    var existing = await _context.Interviews.Include(i => i.Users).FirstOrDefaultAsync(i =>
                        i.CandidateId == candidate_id &&
                        i.JobId == request.job_id &&
                        i.RoundNumber == request.round_number);

                    if (existing != null)
                    {
                        existing.StartTime = start_time;
                        existing.EndTime = end_time;
                        existing.Mode = mode;
                        existing.LocationOrLink = locationOrLink;
                        existing.ScheduledBy = request.scheduled_by;
                        existing.InterviewTypeId = request.interview_type_id;
                        existing.RoundNumber = request.round_number;
                        existing.AllowFeedback = request.accessTo.ToString();
                        existing.JobId = request.job_id;
                        existing.Users.Clear();
                        existing.Status = "Rescheduled";
                        var userIds = panel_members.ToList();
                        var users = await _context.Users
                                                 .Where(u => userIds.Contains(u.UserId))
                                                 .ToListAsync();
                        foreach(var user in users)
                        {
                            existing.Users.Add(user);
                        }
                    }
                    else
                    {
                        var userIds = panel_members.ToList();
                        var users = await _context.Users
                                                 .Where(u => userIds.Contains(u.UserId))
                                                 .ToListAsync();
                        var new_interview = new Interview
                        {
                            StartTime = start_time,
                            EndTime = end_time,
                            Status = "Scheduled",
                            AllowFeedback = request.accessTo.ToString(),
                            RoundNumber = request.round_number,
                            LocationOrLink = locationOrLink,
                            CandidateId = candidate_id,
                            JobId = request.job_id,
                            ScheduledBy = request.scheduled_by,
                            InterviewTypeId = request.interview_type_id,
                            Mode = mode,
                            Users = users,
                        };

                        _context.Interviews.Add(new_interview);
                    }

                    candidate_index++;
                }
            }

            job.Scheduled = "Scheduled";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InterviewDtos.ListOfRoundsRes> FetchInterviewRounds(int job_id)
        {
            var job = await _context.Jobs.AnyAsync(j => j.JobId == job_id);
            if(!job)
                throw new NullReferenceException("Job does not exist");

            var data = await _context.Interviews
                    .Where(i => i.JobId == job_id)
                    .GroupBy(i => new
                    {
                        i.RoundNumber,
                        i.InterviewType.InterviewRoundName,
                        i.InterviewType.ProcessDescreption
                    })
                    .Select(g => new InterviewDtos.ListOfRounds
                    {
                        round_number = g.Key.RoundNumber,
                        interview_round_name = g.Key.InterviewRoundName,
                        process_descreption = g.Key.ProcessDescreption,
                        name = g.FirstOrDefault().ScheduledByUser.Name,
                        interview_date = g.FirstOrDefault().StartTime
                    })
                    .OrderBy(r => r.round_number)
                    .ToListAsync();

            return new InterviewDtos.ListOfRoundsRes
            {
                job_id = job_id,
                roundData = data
            };
        }

        public async Task<List<SkillDtos.SkillDto>> FetchJobSkills(int job_id)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(i => i.JobId == job_id);
            if (job==null)
                throw new NullReferenceException("Job does not exist");

            var skills = await _context.Jobs_Skills.Where(j => j.JobId == job_id).Include(j => j.Skill).Select(r => new SkillDtos.SkillDto
            {
                skill_id = r.SkillId,
                skill_name = r.Skill.SkillName
            }).ToListAsync();

            return skills;
        }

        public async Task<List<InterviewDtos.InterviewerInfo>> GetAllInterviewers(int job_id)
        {
            if (!await _context.Jobs.AnyAsync(j => j.JobId == job_id))
                throw new NullReferenceException("Job does not exist");

            var interviewers = await _context.Interviews
                .Where(i => i.JobId == job_id)
                .SelectMany(i => i.Users)
                .GroupBy(u => new { u.UserId, u.Name }) // group by user
                .Select(g => new InterviewDtos.InterviewerInfo
                {
                    user_id = g.Key.UserId,
                    name = g.Key.Name,
                    role = g.SelectMany(u => u.Roles)
                            .Select(r => r.RoleName)
                            .Distinct()
                            .ToList() // collect all roles per user
                })
                .ToListAsync();


            return interviewers;
        }

        public async Task UpdateCandidateInterviewStatus(int interview_id, string status, string user_id)
        {
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.InterviewId == interview_id);
            if (interview == null) throw new NullReferenceException("Interview does not exist");

            if(!(status == "Selected" || status == "Rejected")) throw new InvalidOperationException("Invalid status");

            interview.Status = status;

            var interview_title = await _context.Interview_Types.FirstOrDefaultAsync(i => i.InterviewTypeId == interview.InterviewTypeId);

            var status_history = new Candidate_Status_History
            {
                Status = status,
                Reason = $"{status} in Round {interview.RoundNumber}, {interview_title?.InterviewRoundName} round.",
                ChangedAt = DateTime.Now,
                CandidateId = interview.CandidateId,
                JobId = interview.JobId,
                ChangedBy = user_id,
            };

            await _context.Candidate_Status_Histories.AddAsync(status_history);

            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewDtos.ListCandidateSheduleRes>> FetchCandidateInterviewShedule(InterviewDtos.ListCandidateSheduleReq req)
        {
            var jobExists = await _context.Jobs
                .AnyAsync(j => j.JobId == req.job_id);

            if (!jobExists)
                throw new NullReferenceException("Job does not exist");

            var user = await _context.Users.AnyAsync(u => u.UserId == req.user_id);
            if (!user)
                throw new NullReferenceException("User does not exist");

            var schedule = await _context.Interviews
                .Where(i => i.JobId == req.job_id &&
                            i.RoundNumber == req.round_number)
                .Select(i => new InterviewDtos.ListCandidateSheduleRes
                {
                    interview_id = i.InterviewId,
                    round_number = i.RoundNumber,
                    location_or_link = i.LocationOrLink,
                    candidate_id = i.CandidateId,
                    job_id = i.JobId,
                    interview_type_id = i.InterviewTypeId,
                    mode = i.Mode,
                    start_time = i.StartTime,
                    end_time = i.EndTime,
                    status = i.Status,
                    scheduled_by = req.user_id,

                    scheduled_by_user = new InterviewDtos.ScheduleByData
                    {
                        user_id = i.ScheduledByUser.UserId,
                        name = i.ScheduledByUser.Name
                    },

                    candidate = new InterviewDtos.CandidateData
                    {
                        candidate_id = i.Candidate.CandidateId,
                        full_name = i.Candidate.FullName,
                        email = i.Candidate.Email
                    },

                    interview_type = new InterviewDtos.Interview_TypeData
                    {
                        interview_type_id = i.InterviewType.InterviewTypeId,
                        interview_round_name = i.InterviewType.InterviewRoundName,
                        process_descreption = i.InterviewType.ProcessDescreption
                    },

                    users = i.Users.Select(u => new InterviewDtos.UserData
                    {
                        user_id = u.UserId,
                        name = u.Name
                    }).ToList()
                })
                .ToListAsync();

            if (schedule == null)
                throw new NullReferenceException("No Interview Scheduled");

            return schedule;
        }

        public async Task<List<InterviewDtos.ListCandidateSheduleRes>> FetchCandidateInterviewSheduleByInterviewer(InterviewDtos.ListCandidateSheduleReq req)
        {
            var jobExists = await _context.Jobs
                .AnyAsync(j => j.JobId == req.job_id);

            if (!jobExists)
                throw new NullReferenceException("Job does not exist");

            var user = await _context.Users.AnyAsync(u => u.UserId == req.user_id);
            if (!user)
                throw new NullReferenceException("User does not exist");

            var schedule = await _context.Interviews
                .Where(i => i.JobId == req.job_id &&
                            i.RoundNumber == req.round_number && i.Users.Any(u => u.UserId == req.user_id))
                .Select(i => new InterviewDtos.ListCandidateSheduleRes
                {
                    interview_id = i.InterviewId,
                    round_number = i.RoundNumber,
                    AccessTo = i.AllowFeedback,
                    location_or_link = i.LocationOrLink,
                    candidate_id = i.CandidateId,
                    job_id = i.JobId,
                    interview_type_id = i.InterviewTypeId,
                    mode = i.Mode,
                    start_time = i.StartTime,
                    end_time = i.EndTime,
                    status = i.Status,
                    scheduled_by = req.user_id,

                    scheduled_by_user = new InterviewDtos.ScheduleByData
                    {
                        user_id = i.ScheduledByUser.UserId,
                        name = i.ScheduledByUser.Name
                    },

                    candidate = new InterviewDtos.CandidateData
                    {
                        candidate_id = i.Candidate.CandidateId,
                        full_name = i.Candidate.FullName,
                        email = i.Candidate.Email
                    },

                    interview_type = new InterviewDtos.Interview_TypeData
                    {
                        interview_type_id = i.InterviewType.InterviewTypeId,
                        interview_round_name = i.InterviewType.InterviewRoundName,
                        process_descreption = i.InterviewType.ProcessDescreption
                    },

                    users = i.Users.Select(u => new InterviewDtos.UserData
                    {
                        user_id = u.UserId,
                        name = u.Name
                    }).ToList()
                })
                .ToListAsync();

            if (schedule == null)
                throw new NullReferenceException("No Interview Scheduled");

            return schedule;
        }

        public async Task<List<CandidateDtos.SelectedCandiadte>> FetchSelectedCandidates(int job_id)
        {
            if (!await _context.Jobs.AnyAsync(i => i.JobId == job_id))
                throw new NullReferenceException("Job does not exist");

            var lastRoundNumber = await _context.Interviews
                .Where(i => i.JobId == job_id)
                .MaxAsync(i => (int?)i.RoundNumber);

            var candidates = await _context.Interviews.Where(i => i.JobId == job_id && i.Status == "Selected" && i.RoundNumber == lastRoundNumber)
                .Select(r => new CandidateDtos.SelectedCandiadte
                {
                    candidate_id = r.CandidateId,
                    full_name = r.Candidate.FullName,
                    email = r.Candidate.Email,
                    doc_upload = r.Candidate.DocUpload
                }).ToListAsync();

            return candidates;
        }

        public async Task<List<JobDtos.ListJobTitle>> CheckCandidateInterviewHistory(int interview_id)
        {
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.InterviewId == interview_id);
            if (interview == null) throw new NullReferenceException("Interivew does not exist");

            var candidateJobs = await _context.Interviews
            .Where(i =>
                i.CandidateId == interview.CandidateId &&
                i.JobId != interview.JobId &&
                i.RoundNumber == 1 &&
                i.StartTime < interview.StartTime)
            .Select(i => new JobDtos.ListJobTitle
            {
                job_id = i.JobId,
                job_title = i.Job.JobTitle
            })
            .Distinct()
            .ToListAsync();

            return candidateJobs;
        }

        public async Task<InterviewDtos.InterviewSkillsRes> FetchSkillDataForInterview(int interview_id)
        {
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.InterviewId == interview_id) ?? throw new NullReferenceException("Interview does not exist");

            var candidateTask = await _context.Candidates.Where(c => c.CandidateId == interview.CandidateId)
                .Select(r => new CandidateDtos.ForInterviewRes
                {
                    candidate_id = r.CandidateId,
                    full_name = r.FullName,
                    email = r.Email,
                    resume_path = r.ResumePath
                }).FirstOrDefaultAsync();

            var jobSkillsTask = await _context.Jobs_Skills
                    .Where(j => j.JobId == interview.JobId)
                    .Include(i => i.Skill)
                    .Select(r => new Jobs_SkillsDtos.InterviewJobSkillRes
                    {
                        skill_id = r.SkillId,
                        skill_type = r.SkillType,
                        skill = new SkillDtos.SkillDto
                        {
                            skill_id = r.Skill.SkillId,
                            skill_name = r.Skill.SkillName,
                        }
                    })
                    .ToListAsync();

            var candidateSkillsTask = await _context.Candidate_Skills
                .Where(c => c.CandidateId == interview.CandidateId)
                .Select(r => new Candidate_SkillDtos.Candidate_SkillDto
                {
                    candidate_skill_id = r.CandidateSkillId,
                    candidate_id = r.CandidateId,
                    years_experience = r.YearsExperience,
                    skill_id = r.SkillId,
                    skill = new SkillDtos.SkillDto
                    {
                        skill_id = r.Skill.SkillId,
                        skill_name = r.Skill.SkillName
                    }
                })
                .ToListAsync();

            return new InterviewDtos.InterviewSkillsRes
            {
                candidate_data = candidateTask,
                job_skills = jobSkillsTask,
                candidate_skills = candidateSkillsTask,
            };
        }

        public async Task<InterviewDtos.UpdateCandidateScheduleReq> UpdateCandidateSchedule(InterviewDtos.UpdateCandidateScheduleReq req)
        {
            if (req.location_or_link.IsNullOrEmpty() || req.mode.IsNullOrEmpty() || req.status.IsNullOrEmpty())
                throw new ArgumentNullException("Invalid input");

            var interview = await _context.Interviews.Include(u => u.Users).FirstOrDefaultAsync(i => i.InterviewId == req.interview_id) ?? throw new NullReferenceException("Interview not found");

            interview.LocationOrLink = req.location_or_link;
            interview.Mode = req.mode;
            interview.Status = req.status;
            interview.ScheduledBy = req.user_id;

            interview.Users.Clear();

            interview.Users = await _context.Users.Where(u => req.users.Contains(u.UserId)).ToListAsync();

            var interview_title = await _context.Interview_Types.FirstOrDefaultAsync(i => i.InterviewTypeId == interview.InterviewTypeId);

            var status_history = new Candidate_Status_History
            {
                Status = req.status,
                Reason = $"{req.status} in Round {interview.RoundNumber}, {interview_title?.InterviewRoundName} round.",
                ChangedAt = DateTime.Now,
                CandidateId = interview.CandidateId,
                JobId = interview.JobId,
                ChangedBy = req.user_id,
            };

            await _context.Candidate_Status_Histories.AddAsync(status_history);

            await _context.SaveChangesAsync();

            return req;
        }

        public async Task<List<InterviewDtos.ListCandidateSheduleRes>?> FetchCandidateInterview(int job_id, string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty()) throw new ArgumentNullException("Invalud input");

            var job = await _context.Jobs.AnyAsync(j => j.JobId == job_id);
            if(!job)
                throw new NullReferenceException("Job does not exist");
            var candidate = await _context.Candidates.AnyAsync(c => c.CandidateId == candidate_id);
            if (!candidate)
                throw new NullReferenceException("Candidate does not exist");

            var schedule = await _context.Interviews
               .Where(i => i.JobId == job_id &&
                           i.CandidateId == candidate_id)
               .Select(i => new InterviewDtos.ListCandidateSheduleRes
               {
                   interview_id = i.InterviewId,
                   round_number = i.RoundNumber,
                   location_or_link = i.LocationOrLink,
                   candidate_id = i.CandidateId,
                   job_id = i.JobId,
                   interview_type_id = i.InterviewTypeId,
                   mode = i.Mode,
                   start_time = i.StartTime,
                   end_time = i.EndTime,
                   status = i.Status,

                   candidate = new InterviewDtos.CandidateData
                   {
                       candidate_id = i.Candidate.CandidateId,
                       full_name = i.Candidate.FullName
                   },

                   interview_type = new InterviewDtos.Interview_TypeData
                   {
                       interview_type_id = i.InterviewType.InterviewTypeId,
                       interview_round_name = i.InterviewType.InterviewRoundName,
                       process_descreption = i.InterviewType.ProcessDescreption
                   },

                   users = i.Users.Select(u => new InterviewDtos.UserData
                   {
                       user_id = u.UserId,
                       name = u.Name
                   }).ToList()
               })
               .ToListAsync();

            if (schedule == null || schedule.Count() == 0)
                return null;
            else
                return schedule;
        }

        public async Task<Boolean> DeleteCandidateInterview(int interview_id)
        {
            var interview = await _context.Interviews
               .Include(i => i.Users)
               .FirstOrDefaultAsync(i => i.InterviewId == interview_id);

            if (interview == null)
                throw new NullReferenceException("Interview Schedule not found");

            interview.Users.Clear();

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteInterviewSchedule(InterviewDtos.DeleteSheduleReq req)
        {

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == req.job_id) ?? throw new NullReferenceException("Job does not exist");

            var interviews = await _context.Interviews
                .Include(i => i.Users)
                .Include(i => i.InterviewFeedbacks)
                .Where(i => i.JobId == req.job_id && i.RoundNumber == req.round_number)
                .ToListAsync();

            if (!interviews.Any())
                throw new NullReferenceException("No Interview Scheduled");

            _context.Interviews.RemoveRange(interviews);
            await _context.SaveChangesAsync();

            var remainingRounds = await _context.Interviews
                .AnyAsync(i => i.JobId == req.job_id);

            if (!remainingRounds)
            {
                job.Scheduled = "Pending";
                await _context.SaveChangesAsync();
            }

            return true;
        }

    }
}