using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public class ScheduleInterviewRequestDto
        {
            public int job_id { get; set; }
            public int round_number { get; set; }
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

            var shortlisted = await _context.Candidate_Status_Histories
                .Join(
                    latestStatus,
                    history => new { history.candidate_id, history.changed_at },
                    latest => new { candidate_id = latest.CandidateId, changed_at = latest.LatestChangedAt },
                    (history, latest) => history
                )
                .Where(r => r.status == "Shortlisted")
                .Select(r => r.candidate_id)
                .ToListAsync();

            return shortlisted;
        }

        public async Task<Boolean> ScheduleInterviews(ScheduleInterviewRequestDto request)
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.job_id == request.job_id) ?? throw new NullReferenceException("Job Does not exist");

            var interview = await _context.Interviews.AnyAsync(i => i.job_id == request.job_id && i.round_number == request.round_number);
            if (interview)
                throw new Exception("Interview Alerady Exist");

            var interviewers_sorted = request.interviewers
                .OrderBy(panel => panel.Last().Equals("Offline", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ToList();

            List<string> candidates = [];
            if(request.result_of == 0)
            {
                candidates = await GetShortlistedCandidates(request.job_id)
                             ?? throw new NullReferenceException("No candidates are shortlisted");
            }
            else
            {
                candidates = await _context.Interviews.Where(i => i.job_id == request.job_id && i.round_number == request.result_of && i.status == "Selected").Select(u => u.candidate_id).ToListAsync();
            }

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

                    var existing = await _context.Interviews.Include(i => i.users).FirstOrDefaultAsync(i =>
                        i.candidate_id == candidate_id &&
                        i.job_id == request.job_id &&
                        i.round_number == request.round_number);

                    if (existing != null)
                    {
                        existing.start_time = start_time;
                        existing.end_time = end_time;
                        existing.mode = mode;
                        existing.location_or_link = locationOrLink;
                        existing.scheduled_by = request.scheduled_by;
                        existing.interview_type_id = request.interview_type_id;
                        existing.round_number = request.round_number;
                        existing.job_id = request.job_id;
                        existing.users.Clear();
                        existing.status = "Rescheduled";
                        var userIds = panel_members.ToList();
                        var users = await _context.Users
                                                 .Where(u => userIds.Contains(u.user_id))
                                                 .ToListAsync();
                        foreach(var user in users)
                        {
                            existing.users.Add(user);
                        }
                    }
                    else
                    {
                        var userIds = panel_members.ToList();
                        var users = await _context.Users
                                                 .Where(u => userIds.Contains(u.user_id))
                                                 .ToListAsync();
                        var new_interview = new Interview
                        {
                            start_time = start_time,
                            end_time = end_time,
                            status = "Scheduled",
                            round_number = request.round_number,
                            location_or_link = locationOrLink,
                            candidate_id = candidate_id,
                            job_id = request.job_id,
                            scheduled_by = request.scheduled_by,
                            interview_type_id = request.interview_type_id,
                            mode = mode,
                            users = users,
                        };

                        _context.Interviews.Add(new_interview);
                    }

                    candidate_index++;
                }
            }

            job.scheduled = "Sheduled";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InterviewDtos.ListOfRoundsRes> FetchInterviewRounds(int job_id)
        {
            var job = await _context.Jobs.AnyAsync(j => j.job_id == job_id);
            if(!job)
                throw new NullReferenceException("Job does not exist");

            var data = await _context.Interviews
                    .Where(i => i.job_id == job_id)
                    .GroupBy(i => new
                    {
                        i.round_number,
                        i.interview_type.interview_round_name,
                        i.interview_type.process_descreption
                    })
                    .Select(g => new InterviewDtos.ListOfRounds
                    {
                        round_number = g.Key.round_number,
                        interview_round_name = g.Key.interview_round_name,
                        process_descreption = g.Key.process_descreption,
                        name = g.FirstOrDefault().scheduled_by_user.name,
                        interview_date = g.FirstOrDefault().start_time
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
            var job = await _context.Jobs.FirstOrDefaultAsync(i => i.job_id == job_id);
            if (job==null)
                throw new NullReferenceException("Job does not exist");

            var skills = await _context.Jobs_Skills.Where(j => j.job_id == job_id).Include(j => j.skill).Select(r => new SkillDtos.SkillDto
            {
                skill_id = r.skill_id,
                skill_name = r.skill.skill_name
            }).ToListAsync();

            return skills;
        }

        public async Task UpdateCandidateInterviewStatus(int interview_id, string status)
        {
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.interview_id == interview_id);
            if (interview == null) throw new NullReferenceException("Interview does not exist");

            if(!(status == "Selected" || status == "Rejected")) throw new InvalidOperationException("Invalid status");

            interview.status = status;

            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewDtos.ListCandidateSheduleRes>> FetchCandidateInterviewShedule(InterviewDtos.ListCandidateSheduleReq req)
        {
            var jobExists = await _context.Jobs
                .AnyAsync(j => j.job_id == req.job_id);

            if (!jobExists)
                throw new NullReferenceException("Job does not exist");

            var user = await _context.Users.AnyAsync(u => u.user_id == req.user_id);
            if (!user)
                throw new NullReferenceException("User does not exist");

            var schedule = await _context.Interviews
                .Where(i => i.job_id == req.job_id &&
                            i.round_number == req.round_number)
                .Select(i => new InterviewDtos.ListCandidateSheduleRes
                {
                    interview_id = i.interview_id,
                    round_number = i.round_number,
                    location_or_link = i.location_or_link,
                    candidate_id = i.candidate_id,
                    job_id = i.job_id,
                    interview_type_id = i.interview_type_id,
                    mode = i.mode,
                    start_time = i.start_time,
                    end_time = i.end_time,
                    status = i.status,
                    scheduled_by = req.user_id,

                    scheduled_by_user = new InterviewDtos.ScheduleByData
                    {
                        user_id = i.scheduled_by_user.user_id,
                        name = i.scheduled_by_user.name
                    },

                    candidate = new InterviewDtos.CandidateData
                    {
                        candidate_id = i.candidate.candidate_id,
                        full_name = i.candidate.full_name,
                        email = i.candidate.email
                    },

                    interview_type = new InterviewDtos.Interview_TypeData
                    {
                        interview_type_id = i.interview_type.interview_type_id,
                        interview_round_name = i.interview_type.interview_round_name,
                        process_descreption = i.interview_type.process_descreption
                    },

                    users = i.users.Select(u => new InterviewDtos.UserData
                    {
                        user_id = u.user_id,
                        name = u.name
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
                .AnyAsync(j => j.job_id == req.job_id);

            if (!jobExists)
                throw new NullReferenceException("Job does not exist");

            var user = await _context.Users.AnyAsync(u => u.user_id == req.user_id);
            if (!user)
                throw new NullReferenceException("User does not exist");

            var schedule = await _context.Interviews
                .Where(i => i.job_id == req.job_id &&
                            i.round_number == req.round_number && i.users.Any(u => u.user_id == req.user_id))
                .Select(i => new InterviewDtos.ListCandidateSheduleRes
                {
                    interview_id = i.interview_id,
                    round_number = i.round_number,
                    location_or_link = i.location_or_link,
                    candidate_id = i.candidate_id,
                    job_id = i.job_id,
                    interview_type_id = i.interview_type_id,
                    mode = i.mode,
                    start_time = i.start_time,
                    end_time = i.end_time,
                    status = i.status,
                    scheduled_by = req.user_id,

                    scheduled_by_user = new InterviewDtos.ScheduleByData
                    {
                        user_id = i.scheduled_by_user.user_id,
                        name = i.scheduled_by_user.name
                    },

                    candidate = new InterviewDtos.CandidateData
                    {
                        candidate_id = i.candidate.candidate_id,
                        full_name = i.candidate.full_name,
                        email = i.candidate.email
                    },

                    interview_type = new InterviewDtos.Interview_TypeData
                    {
                        interview_type_id = i.interview_type.interview_type_id,
                        interview_round_name = i.interview_type.interview_round_name,
                        process_descreption = i.interview_type.process_descreption
                    },

                    users = i.users.Select(u => new InterviewDtos.UserData
                    {
                        user_id = u.user_id,
                        name = u.name
                    }).ToList()
                })
                .ToListAsync();

            if (schedule == null)
                throw new NullReferenceException("No Interview Scheduled");

            return schedule;
        }

        public async Task<List<JobDtos.ListJobTitle>> CheckCandidateInterviewHistory(int interview_id)
        {
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.interview_id == interview_id);
            if (interview == null) throw new NullReferenceException("Interivew does not exist");

            var candidateJobs = await _context.Interviews
            .Where(i =>
                i.candidate_id == interview.candidate_id &&
                i.job_id != interview.job_id &&
                i.round_number == 1 &&
                i.start_time < DateTime.Now)
            .Select(i => new JobDtos.ListJobTitle
            {
                job_id = i.job_id,
                job_title = i.job.job_title
            })
            .Distinct()
            .ToListAsync();

            return candidateJobs;
        }

        public async Task<InterviewDtos.InterviewSkillsRes> FetchSkillDataForInterview(int interview_id)
        {
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.interview_id == interview_id) ?? throw new NullReferenceException("Interview does not exist");

            var candidateTask = await _context.Candidates.Where(c => c.candidate_id == interview.candidate_id)
                .Select(r => new CandidateDtos.ForInterviewRes
                {
                    candidate_id = r.candidate_id,
                    full_name = r.full_name,
                    email = r.email,
                    resume_path = r.resume_path
                }).FirstOrDefaultAsync();

            var jobSkillsTask = await _context.Jobs_Skills
                    .Where(j => j.job_id == interview.job_id)
                    .Include(i => i.skill)
                    .Select(r => new Jobs_SkillsDtos.InterviewJobSkillRes
                    {
                        skill_id = r.skill_id,
                        skill_type = r.skill_type,
                        skill = new SkillDtos.SkillDto
                        {
                            skill_id = r.skill.skill_id,
                            skill_name = r.skill.skill_name,
                        }
                    })
                    .ToListAsync();

            Console.WriteLine(jobSkillsTask.ToString());

            var candidateSkillsTask = await _context.Candidate_Skills
                .Where(c => c.candidate_id == interview.candidate_id)
                .Select(r => new Candidate_SkillDtos.Candidate_SkillDto
                {
                    candidate_skill_id = r.candidate_skill_id,
                    candidate_id = r.candidate_id,
                    years_experience = r.years_experience,
                    skill_id = r.skill_id,
                    skill = new SkillDtos.SkillDto
                    {
                        skill_id = r.skill.skill_id,
                        skill_name = r.skill.skill_name
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

            var interview = await _context.Interviews.Include(u => u.users).FirstOrDefaultAsync(i => i.interview_id == req.interview_id) ?? throw new NullReferenceException("Interview not found");

            interview.location_or_link = req.location_or_link;
            interview.mode = req.mode;
            interview.status = req.status;
            interview.scheduled_by = req.user_id;

            interview.users.Clear();

            interview.users = await _context.Users.Where(u => req.users.Contains(u.user_id)).ToListAsync();

            await _context.SaveChangesAsync();

            return req;
        }

        public async Task<List<InterviewDtos.ListCandidateSheduleRes>?> FetchCandidateInterview(InterviewDtos.CandidateInterviewReq req)
        {
            if (req.candidate_id.IsNullOrEmpty()) throw new ArgumentNullException("Invalud input");

            var job = await _context.Jobs.AnyAsync(j => j.job_id == req.job_id);
            if(!job)
                throw new NullReferenceException("Job does not exist");
            var candidate = await _context.Candidates.AnyAsync(c => c.candidate_id == req.candidate_id);
            if (!candidate)
                throw new NullReferenceException("Candidate does not exist");

            var schedule = await _context.Interviews
               .Where(i => i.job_id == req.job_id &&
                           i.candidate_id == req.candidate_id)
               .Select(i => new InterviewDtos.ListCandidateSheduleRes
               {
                   interview_id = i.interview_id,
                   round_number = i.round_number,
                   location_or_link = i.location_or_link,
                   candidate_id = i.candidate_id,
                   job_id = i.job_id,
                   interview_type_id = i.interview_type_id,
                   mode = i.mode,
                   start_time = i.start_time,
                   end_time = i.end_time,
                   status = i.status,

                   candidate = new InterviewDtos.CandidateData
                   {
                       candidate_id = i.candidate.candidate_id,
                       full_name = i.candidate.full_name
                   },

                   interview_type = new InterviewDtos.Interview_TypeData
                   {
                       interview_type_id = i.interview_type.interview_type_id,
                       interview_round_name = i.interview_type.interview_round_name,
                       process_descreption = i.interview_type.process_descreption
                   },

                   users = i.users.Select(u => new InterviewDtos.UserData
                   {
                       user_id = u.user_id,
                       name = u.name
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
               .Include(i => i.users)
               .FirstOrDefaultAsync(i => i.interview_id == interview_id);

            if (interview == null)
                throw new NullReferenceException("Interview Schedule not found");

            interview.users.Clear();

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteInterviewSchedule(InterviewDtos.DeleteSheduleReq req)
        {

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.job_id == req.job_id) ?? throw new NullReferenceException("Job does not exist");

            var interviews = await _context.Interviews
                .Include(i => i.users)
                .Include(i => i.Interview_Feedbacks)
                .Where(i => i.job_id == req.job_id && i.round_number == req.round_number)
                .ToListAsync();

            if (!interviews.Any())
                throw new NullReferenceException("No Interview Scheduled");

            _context.Interviews.RemoveRange(interviews);
            await _context.SaveChangesAsync();

            var remainingRounds = await _context.Interviews
                .AnyAsync(i => i.job_id == req.job_id);

            if (!remainingRounds)
            {
                job.scheduled = "Pending";
                await _context.SaveChangesAsync();
            }

            return true;
        }

    }
}