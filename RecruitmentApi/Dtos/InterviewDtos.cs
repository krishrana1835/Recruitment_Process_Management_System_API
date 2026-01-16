using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    public class InterviewDtos
    {
        public class InterviewDtos_Candidate
        {
            public int interview_id { get; set; }
            public int job_id { get; set; }
            public int round_number { get; set; }
            public string round_title { get; set; } = null!;
            public string status { get; set; } = null!;
            public JobDtos.JobDto_Candidate job { get; set; }
        }

        public class DeleteSheduleReq
        {
            public int job_id { get; set; }

            public int round_number { get; set; }
        }

        public class ListOfRoundsRes
        {
            public int job_id { get; set; }
            public List<ListOfRounds> roundData { get; set; } = new();
        }

        public class InterviewerInfo
        {
            public string user_id { get; set; } = null!;
            public string name { get; set; } = null!;
            public List<string> role { get; set; } = new();
        }
        public class ListOfRounds
        {
            public int round_number { get; set; }
            public string interview_round_name { get; set; } = null!;
            public string? process_descreption { get; set; }
            public string name { get; set; } = null!;
            public DateTime interview_date { get; set; }
        }

        public class ListCandidateSheduleReq
        {
            public int job_id { get; set; }
            public int round_number { get; set; }
            public string user_id { get; set; } = null!;
        }

        public class ListCandidateSheduleRes
        {
            public int interview_id { get; set; }

            public int round_number { get; set; }

            public string AccessTo { get; set; } = null!;
            public string location_or_link { get; set; } = null!;

            public string candidate_id { get; set; } = null!;

            public int job_id { get; set; }

            public int interview_type_id { get; set; }

            public string mode { get; set; } = null!;

            public DateTime start_time { get; set; }

            public DateTime end_time { get; set; }

            public string status { get; set; } = null!;

            public string scheduled_by { get; set; } = null!;

            public virtual CandidateData candidate { get; set; } = null!;

            public virtual Interview_TypeData interview_type { get; set; } = null!;

            public virtual ICollection<UserData> users { get; set; } = [];

            public virtual ScheduleByData scheduled_by_user { get; set; } = null!;

        }

        public class ScheduleByData
        {
            public string user_id { get; set; } = null!;
            public string name { get; set; } = null!;
        }

        public class CandidateData
        {
            public string candidate_id { get; set; } = null!;
            public string full_name { get; set; } = null!;
            public string email { get; set; } = null!;
        }

        public class Interview_TypeData
        {
            public int interview_type_id { get; set; }

            public string interview_round_name { get; set; } = null!;

            public string? process_descreption { get; set; }
        }

        public class UserData
        {
            public string user_id { get; set; } = null!;
            public string name { get; set; } = null!;
        }
        public class UpdateCandidateScheduleReq
        {
            public int interview_id { get; set; }
            public string location_or_link { get; set; } = null!;
            public string mode { get; set; } = null!;
            public string status { get; set; } = null!;
            public string user_id { get; set; } = null!;
            public List<string> users { get; set; }
        }

        public class InterviewStatusUpdateReq
        {
            public int interview_id { get; set; }
            public string status { get; set; } = null!;
            public string user_id { get; set;} = null!;
        }

        public class InterviewSkillsRes
        {
            public CandidateDtos.ForInterviewRes candidate_data { get; set; } = new();
            public List<Jobs_SkillsDtos.InterviewJobSkillRes> job_skills { get; set; } = new();
            public List<Candidate_SkillDtos.Candidate_SkillDto> candidate_skills { get; set; } = new();
        }
    }
}
