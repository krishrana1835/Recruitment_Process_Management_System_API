using System.Security.Cryptography.X509Certificates;
using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    /// <summary>
    /// Represents a data transfer object for job-related information.
    /// </summary>
    public class JobDtos
    {   
        /// <summary>
        /// Represents a single job.
        /// </summary>
        public class JobDto
        {
            /// <summary>
            /// Gets or sets the unique identifier for the job.
            /// </summary>
            public int job_id { get; set; }

            /// <summary>
            /// Gets or sets the title of the job.
            /// </summary>
            public string job_title { get; set; } = null!;

            /// <summary>
            /// Gets or sets the description of the job.
            /// </summary>
            public string job_description { get; set; } = null!;

            /// <summary>
            /// Gets or sets the date and time when the job was created.
            /// </summary>
            public DateTime created_at { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier for the job's status.
            /// </summary>
            public int status_id { get; set; }

            /// <summary>
            /// Gets or sets the status of the job.
            /// </summary>
            public virtual Jobs_StatusDtos.Jobs_StatusDto status { get; set; } = null!;
        }

        public class JobDto_Candidate
        {
            /// <summary>
            /// Gets or sets the unique identifier for the job.
            /// </summary>
            public int job_id { get; set; }

            /// <summary>
            /// Gets or sets the title of the job.
            /// </summary>
            public string job_title { get; set; } = null!;
        }

        public class ListAllJobs
        {
            /// <summary>
            /// Gets or sets the unique identifier for the job.
            /// </summary>
            public int job_id { get; set; }

            /// <summary>
            /// Gets or sets the title of the job.
            /// </summary>
            public string job_title { get; set; } = null!;

            /// <summary>
            /// Gets or sets the description of the job.
            /// </summary>
            public string job_description { get; set; } = null!;

            /// <summary>
            /// Gets or sets the date and time when the job was created.
            /// </summary>
            public DateTime created_at { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier for the job's status.
            /// </summary>
            public int status_id { get; set; }

            /// <summary>
            /// Gets or sets the status of the job.
            /// </summary>
            /// 

            public string scheduled { get; set; } = null!;
            public virtual Jobs_StatusDtos.ListAllJobs status { get; set; } = null!;
        }

        public class CreateJobDto
        {

            /// <summary>
            /// Gets or sets the title of the job.
            /// </summary>
            public string job_title { get; set; } = null!;

            /// <summary>
            /// Gets or sets the description of the job.
            /// </summary>
            public string job_description { get; set; } = null!;


            public string created_by { get; set; } = null!;

            public virtual ICollection<Jobs_SkillsDtos.AddJobs_SkillDto> Jobs_Skills { get; set; }

            /// <summary>
            /// Gets or sets the status of the job.
            /// </summary>
            public virtual Jobs_StatusDtos.CreateJobStatusDto status { get; set; } = null!;
        }

        public class ListJobStatus
        {
            public string job_title { get; set; } = null!;
            public virtual Jobs_StatusDtos.ListJobStatus status { get; set; } = null!;
        }

        public class ListJobTitle
        {
            public int job_id { get; set; }
            public string job_title { get; set; } = null!;
            public string sheduled { get; set; } = null!;
        }

        public class JobTitle
        {
            public string job_title { get; set; } = null!;

        }

        public class UpdateJobDto : CreateJobDto
        {
            public int job_id { get; set; }
        }
    }
}
