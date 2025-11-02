namespace RecruitmentApi.Dtos
{
    /// <summary>
    /// Represents a data transfer object for job status information.
    /// </summary>
    public class Jobs_StatusDtos
    {
        /// <summary>
        /// Represents a single job status entry.
        /// </summary>
        public class Jobs_StatusDto
        {
            /// <summary>
            /// Gets or sets the unique identifier for the status.
            /// </summary>
            public int status_id { get; set; }

            /// <summary>
            /// Gets or sets the status description.
            /// </summary>
            public string status { get; set; } = null!;

            /// <summary>
            /// Gets or sets the reason for the status change.
            /// </summary>
            public string? reason { get; set; }

            /// <summary>
            /// Gets or sets the timestamp when the status was changed.
            /// </summary>
            public DateTime changed_at { get; set; }

            /// <summary>
            /// Gets or sets the user who changed the status.
            /// </summary>
            public string changed_by { get; set; } = null!;
        }
        public class ListAllJobs
        {
            /// <summary>
            /// Gets or sets the unique identifier for the status.
            /// </summary>
            public int status_id { get; set; }

            /// <summary>
            /// Gets or sets the status description.
            /// </summary>
            public string status { get; set; } = null!;
        }
        public class CreateJobStatusDto
        {
            public string status { get; set; } = null!;

            public string? reason { get; set; }
            public string changed_by { get; set; } = null!;
        }

        public class ListJobStatus
        {
            public string status { get; set; } = null!;
        }
    }
}