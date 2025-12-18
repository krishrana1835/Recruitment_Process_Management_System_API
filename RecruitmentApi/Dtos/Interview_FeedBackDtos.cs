using RecruitmentApi.Dtos;

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

    public class InterveiwFeedbackCard
    {
        public int concept_rating { get; set; }
        public string user_id { get; set; } = null!;
        public int technical_rating { get; set; }
        public int candidate_skill_id { get; set; }

        public Candidate_SkillDtos.Candidate_SkillDto Candidaete_Skill { get; set; } = new();
    }

    public class SkillReviewDto
    {
        public int skill_id { get; set; }
        public string skill_name { get; set; } = string.Empty;
        public string skill_type { get; set; } = string.Empty;

        public SkillReviewDataDto review { get; set; } = new SkillReviewDataDto();
    }

    public class SkillReviewDataDto
    {
        public int yearsOfExperience { get; set; } = 0;
        public int conceptRating { get; set; }
        public int technicalRating { get; set; }
        public string comments { get; set; } = "";
    }

    public class InterviewSkillSubmissionDto
    {
        public int interview_id { get; set; }
        public string user_id { get; set; } = null!;

        public string candidate_id { get; set; } = null!;

        public float total_score { get; set; }

        public List<SkillReviewDto> extra_skills { get; set; } = new();
        public List<SkillReviewDto> preferred_skills { get; set; } = new();
        public List<SkillReviewDto> required_skills { get; set; } = new();
    }

    public class GetFeedbackReq
    {
        public string user_id { get; set; } = null!;
        public int interview_id { get; set; }
    }
}