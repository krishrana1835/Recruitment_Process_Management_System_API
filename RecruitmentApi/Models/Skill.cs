using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Skill
{
    public int SkillId { get; set; }

    public string SkillName { get; set; } = null!;

    public virtual ICollection<Candidate_Skill> CandidateSkills { get; set; } = new List<Candidate_Skill>();

    public virtual ICollection<Jobs_Skill> JobsSkills { get; set; } = new List<Jobs_Skill>();
}
