using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    public class JobDtos
    {   
        public class JobDto
        {
            public int job_id { get; set; }

            public string job_title { get; set; } = null!;

            public string job_description { get; set; } = null!;

            public DateTime created_at { get; set; }

            public int status_id { get; set; }

            public virtual Jobs_StatusDtos.Jobs_StatusDto status { get; set; } = null!;
        }
    }
}
