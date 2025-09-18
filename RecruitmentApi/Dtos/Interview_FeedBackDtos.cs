using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    public class Interview_FeedBackDtos
    {
        public class Interview_FeedbackDto
        {
            public int feedback_id { get; set; }

            public int rating { get; set; }

            public string comments { get; set; } = null!;

            public DateTime feedback_at { get; set; }

            public int interview_id { get; set; }

            public int skill_id { get; set; }
        }
    }
}
