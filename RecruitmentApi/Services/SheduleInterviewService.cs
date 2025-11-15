using System;
using System.Collections.Generic;
using System.Linq;
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
            public List<string> candidates { get; set; } = new();
            public List<List<string>> interviewers { get; set; } = new();
            public string scheduled_by { get; set; } = string.Empty;
            public string location_or_link { get; set; } = "TBD";
            public int interview_type_id { get; set; } = 1;
        }

        public class ScheduleInterviewResponseDto
        {
            public string candidate_id { get; set; } = string.Empty;
            public int round_number { get; set; }
            public int job_id { get; set; }
            public string mode { get; set; } = string.Empty;
            public string start_time { get; set; } = string.Empty;
            public string end_time { get; set; } = string.Empty;
            public List<string> assigned_interviewers { get; set; } = new();
            public string status { get; set; } = "Scheduled";
        }

        // ----------------- Core Logic -----------------
        public List<ScheduleInterviewResponseDto> ScheduleInterviews(ScheduleInterviewRequestDto request)
        {
            var results = new List<ScheduleInterviewResponseDto>();

            // Sort panels: Offline first, then Online
            var interviewers_sorted = request.interviewers
                .OrderBy(panel => panel.Last().Equals("Offline", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ToList();

            int panels = interviewers_sorted.Count;
            int total_candidates = request.candidates.Count;
            int total_slots = (int)Math.Ceiling(total_candidates / (double)panels);

            int candidate_index = 0;

            for (int slot = 0; slot < total_slots; slot++)
            {
                var start_time = request.scheduled_start_time.AddMinutes(slot * request.duration_per_interview);
                var end_time = start_time.AddMinutes(request.duration_per_interview);

                for (int panel_id = 0; panel_id < panels; panel_id++)
                {
                    if (candidate_index >= total_candidates)
                        break;

                    string candidate_id = request.candidates[candidate_index];
                    var panel = interviewers_sorted[panel_id];
                    var panel_members = panel.Take(panel.Count - 1).ToList();
                    string mode = panel.Last();

                    // Check if interview already exists (reschedule)
                    var existing = _context.Interviews
                        .FirstOrDefault(i =>
                            i.candidate_id == candidate_id &&
                            i.job_id == request.job_id &&
                            i.round_number == request.round_number);

                    string status;

                    if (existing != null)
                    {
                        existing.mode = mode;
                        existing.location_or_link = request.location_or_link;
                        existing.scheduled_by = request.scheduled_by;
                        existing.interview_type_id = request.interview_type_id;
                        existing.round_number = request.round_number;
                        existing.job_id = request.job_id;

                        status = "Rescheduled";
                        _context.Interviews.Update(existing);
                    }
                    else
                    {
                        var new_interview = new Interview
                        {
                            round_number = request.round_number,
                            location_or_link = request.location_or_link,
                            candidate_id = candidate_id,
                            job_id = request.job_id,
                            scheduled_by = request.scheduled_by,
                            interview_type_id = request.interview_type_id,
                            mode = mode
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
                        start_time = start_time.ToString("yyyy-MM-dd HH:mm"),
                        end_time = end_time.ToString("yyyy-MM-dd HH:mm"),
                        assigned_interviewers = panel_members,
                        status = status
                    });

                    candidate_index++;
                }
            }

            //_context.SaveChanges();
            return results;
        }
    }
}