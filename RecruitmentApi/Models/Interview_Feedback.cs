using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Interview_Feedback
{
    public int FeedbackId { get; set; }

    public int ConceptRating { get; set; }

    public int TechnicalRating { get; set; }

    public string Comments { get; set; } = null!;

    public DateTime FeedbackAt { get; set; }

    public int InterviewId { get; set; }

    public string UserId { get; set; } = null!;

    public int CandidateSkillId { get; set; }

    public virtual Interview Interview { get; set; } = null!;

    public virtual Candidate_Skill CandidateSkill { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
