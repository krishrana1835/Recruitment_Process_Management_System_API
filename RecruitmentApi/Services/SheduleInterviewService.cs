//using RecruitmentApi.Data;

//namespace RecruitmentApi.Services
//{
//    public class SheduleInterviewService
//    {
//        public readonly AppDbContext _context;

//        public SheduleInterviewService(AppDbContext context)
//        {
//            _context = context;
//        }

//        public class ScheduleInterviewRequestDto
//        {
//            public int job_id { get; set; }
//            public int round_number { get; set; }
//            public DateTime ScheduledStartTime { get; set; }
//            public int DurationPerInterview { get; set; }
//            public List<string> Candidates { get; set; } = new();
//            public List<List<string>> Interviewers { get; set; } = new();
//        }

//        public class ScheduleInterviewResponseDto
//        {
//            public string CandidateId { get; set; } = string.Empty;
//            public int RoundNumber { get; set; }
//            public int JobId { get; set; }
//            public string Mode { get; set; } = string.Empty;
//            public string StartTime { get; set; } = string.Empty;
//            public string EndTime { get; set; } = string.Empty;
//            public List<string> AssignedInterviewers { get; set; } = new();
//            public string Status { get; set; } = "Scheduled";
//        }
//        public class AssignmentRecord
//        {
//            public int AssignmentId { get; set; }
//            public string InterviewerId { get; set; } = string.Empty;
//            public int PanelId { get; set; }
//        }

//        public class InterviewRecord
//        {
//            public string CandidateId { get; set; } = string.Empty;
//            public int JobId { get; set; }
//            public int RoundId { get; set; }
//            public DateTime StartTime { get; set; }
//            public DateTime EndTime { get; set; }
//            public int PanelId { get; set; }
//            public string Mode { get; set; } = string.Empty;
//            public string Status { get; set; } = "Scheduled";
//        }


//        public class InterviewSchedulerService
//        {
//            private readonly List<InterviewRecord> _interviews = new();
//            private readonly List<AssignmentRecord> _assignments = new();

//            public List<ScheduleInterviewResponseDto> ScheduleInterviews(ScheduleInterviewRequestDto request)
//            {
//                var results = new List<ScheduleInterviewResponseDto>();

//                // Sort panels: Offline first, then Online
//                var interviewersSorted = request.Interviewers
//                    .OrderBy(panel => panel.Last().Equals("Offline", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
//                    .ToList();

//                int panels = interviewersSorted.Count;
//                int totalCandidates = request.Candidates.Count;
//                int totalSlots = (int)Math.Ceiling(totalCandidates / (double)panels);

//                int candidateIndex = 0;

//                for (int slot = 0; slot < totalSlots; slot++)
//                {
//                    var startTime = request.ScheduledStartTime.AddMinutes(slot * request.DurationPerInterview);
//                    var endTime = startTime.AddMinutes(request.DurationPerInterview);

//                    for (int panelId = 0; panelId < panels; panelId++)
//                    {
//                        if (candidateIndex >= totalCandidates)
//                            break;

//                        string candidateId = request.Candidates[candidateIndex];
//                        var panel = interviewersSorted[panelId];
//                        var panelMembers = panel.Take(panel.Count - 1).ToList();
//                        string mode = panel.Last();

//                        // Check if interview exists (reschedule case)
//                        var existing = _interviews.FirstOrDefault(i =>
//                            i.CandidateId == candidateId &&
//                            i.JobId == request.JobId &&
//                            i.RoundId == request.RoundId);

//                        InterviewRecord interview;
//                        if (existing != null)
//                        {
//                            existing.StartTime = startTime;
//                            existing.EndTime = endTime;
//                            existing.Status = "Rescheduled";
//                            existing.PanelId = panelId + 1;
//                            existing.Mode = mode;
//                            interview = existing;
//                        }
//                        else
//                        {
//                            interview = new InterviewRecord
//                            {
//                                CandidateId = candidateId,
//                                JobId = request.JobId,
//                                RoundId = request.RoundId,
//                                StartTime = startTime,
//                                EndTime = endTime,
//                                PanelId = panelId + 1,
//                                Mode = mode,
//                                Status = "Scheduled"
//                            };
//                            _interviews.Add(interview);
//                        }

//                        // Assign interviewers to panel
//                        foreach (var iid in panelMembers)
//                        {
//                            _assignments.Add(new AssignmentRecord
//                            {
//                                AssignmentId = _assignments.Count + 1,
//                                InterviewerId = iid,
//                                PanelId = panelId + 1
//                            });
//                        }

//                        results.Add(new ScheduleInterviewResponseDto
//                        {
//                            CandidateId = candidateId,
//                            RoundNumber = request.RoundId,
//                            JobId = request.JobId,
//                            Mode = mode,
//                            StartTime = startTime.ToString("yyyy-MM-dd HH:mm"),
//                            EndTime = endTime.ToString("yyyy-MM-dd HH:mm"),
//                            AssignedInterviewers = panelMembers,
//                            Status = interview.Status
//                        });

//                        candidateIndex++;
//                    }
//                }

//                return results;
//            }
//        }
//}
