/// <summary>
    /// Represents a data transfer object for interview feedback.
    /// </summary>
    public class Interview_FeedBackDtos
    {
        /// <summary>
        /// Represents a single interview feedback entry.
        /// </summary>
        public class Interview_FeedbackDto
        {
            /// <summary>
            /// Gets or sets the unique identifier for the feedback.
            /// </summary>
            public int feedback_id { get; set; }

            /// <summary>
            /// Gets or sets the rating given in the feedback.
            /// </summary>
            public int rating { get; set; }

            /// <summary>
            /// Gets or sets the comments made in the feedback.
            /// </summary>
            public string comments { get; set; } = null!;

            /// <summary>
            /// Gets or sets the timestamp when the feedback was provided.
            /// </summary>
            public DateTime feedback_at { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier of the interview associated with the feedback.
            /// </summary>
            public int interview_id { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier of the skill being evaluated in the feedback.
            /// </summary>
            public int candidate_skill_id { get; set; }
        }
    }
