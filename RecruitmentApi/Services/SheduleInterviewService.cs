using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Data;
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

        // ----------------- DTOs -----------------
        public class ScheduleInterviewRequestDto
        {
            public int job_id { get; set; }
            public int round_number { get; set; }
            public DateTime scheduled_start_time { get; set; }
            public int duration_per_interview { get; set; }

            // Example: ["IV101","IV102","Online"]
            public List<List<string>> interviewers { get; set; } = new();

            // ⚠️ Direct mapping: panel 0 → panel_links[0], panel 1 → panel_links[1]
            public List<string>? panel_links { get; set; } = new();

            public string scheduled_by { get; set; } = string.Empty;

            public string? location { get; set; } // For offline only  
            // NOTE: "link" is unused now, because we use panel_links only
            public string? link { get; set; }

            public int interview_type_id { get; set; }
        }

        public class ScheduleInterviewResponseDto
        {
            public string candidate_id { get; set; } = string.Empty;
            public int round_number { get; set; }
            public int job_id { get; set; }
            public string mode { get; set; } = string.Empty;
            public DateTime start_time { get; set; }
            public DateTime end_time { get; set; }
            public List<string> assigned_interviewers { get; set; } = new();
            public string status { get; set; } = "Scheduled";
            public string location_or_link { get; set; } = "TBD";
            public int interview_type_id { get; set; }
        }

        // ----------------- Helper: Get Shortlisted Candidates -----------------
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

        // ----------------- Core Logic: Schedule Interviews -----------------
        public async Task<List<ScheduleInterviewResponseDto>> ScheduleInterviews(ScheduleInterviewRequestDto request)
        {
            var results = new List<ScheduleInterviewResponseDto>();

            // Sort panels: Offline first, then Online
            var interviewers_sorted = request.interviewers
                .OrderBy(panel => panel.Last().Equals("Offline", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ToList();

            var candidates = await GetShortlistedCandidates(request.job_id)
                             ?? throw new NullReferenceException("No candidates are shortlisted");

            int panels = interviewers_sorted.Count;
            int total_candidates = candidates.Count;
            int total_slots = (int)Math.Ceiling(total_candidates / (double)panels);

            int candidate_index = 0;

            // Ensure panel_links exist
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

                    // ----------------- STRICT LINK HANDLING -----------------
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
                    // ---------------------------------------------------------

                    var existing = await _context.Interviews.FirstOrDefaultAsync(i =>
                        i.candidate_id == candidate_id &&
                        i.job_id == request.job_id &&
                        i.round_number == request.round_number);

                    string status;

                    if (existing != null)
                    {
                        existing.mode = mode;
                        existing.location_or_link = locationOrLink;
                        existing.scheduled_by = request.scheduled_by;
                        existing.interview_type_id = request.interview_type_id;
                        existing.round_number = request.round_number;
                        existing.job_id = request.job_id;
                        existing.users.Clear();
                        var userIds = panel_members.ToList();
                        var users = await _context.Users
                                                 .Where(u => userIds.Contains(u.user_id))
                                                 .ToListAsync();
                        foreach(var user in users)
                        {
                            existing.users.Add(user);
                        }

                        status = "Rescheduled";
                    }
                    else
                    {
                        var userIds = panel_members.ToList();
                        var users = await _context.Users
                                                 .Where(u => userIds.Contains(u.user_id))
                                                 .ToListAsync();
                        var new_interview = new Interview
                        {
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
                        status = "Scheduled";
                    }


                    results.Add(new ScheduleInterviewResponseDto
                    {
                        candidate_id = candidate_id,
                        round_number = request.round_number,
                        job_id = request.job_id,
                        mode = mode,
                        start_time = start_time,
                        end_time = end_time,
                        assigned_interviewers = panel_members,
                        status = status,
                        location_or_link = locationOrLink,
                        interview_type_id = request.interview_type_id
                    });

                    candidate_index++;
                }
            }
            await _context.SaveChangesAsync();

            return results;
        }

    }
}