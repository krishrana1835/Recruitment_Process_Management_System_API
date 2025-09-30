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
    }
}
