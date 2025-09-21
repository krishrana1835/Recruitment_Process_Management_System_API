/// <summary>
    /// Represents a data transfer object for candidate reviews.
    /// </summary>
    public class Candidate_ReviewDtos
    {
        /// <summary>
        /// Represents a single candidate review.
        /// </summary>
        public class Candidate_ReviewDto
        {
            /// <summary>
            /// Gets or sets the unique identifier for the review.
            /// </summary>
            public int review_id { get; set; }

            /// <summary>
            /// Gets or sets the comments made during the review.
            /// </summary>
            public string comments { get; set; } = null!;

            /// <summary>
            /// Gets or sets the timestamp when the review was conducted.
            /// </summary>
            public DateTime reviewed_at { get; set; }

            /// <summary>
            /// Gets or sets the unique identifier of the candidate being reviewed.
            /// </summary>
            public string candidate_id { get; set; } = null!;

            /// <summary>
            /// Gets or sets the unique identifier of the job associated with the review.
            /// </summary>
            public int job_id { get; set; }
        }
    }
