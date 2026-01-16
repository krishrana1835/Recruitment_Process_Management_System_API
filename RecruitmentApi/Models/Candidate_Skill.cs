using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Candidate_Skill
{
    public int CandidateSkillId { get; set; }

    public int YearsExperience { get; set; }

    public int SkillId { get; set; }

    public string CandidateId { get; set; } = null!;

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual ICollection<Interview_Feedback> InterviewFeedbacks { get; set; } = new List<Interview_Feedback>();

    public virtual Skill Skill { get; set; } = null!;
}
