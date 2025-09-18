using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Candidate_Skill
{
    public int candidate_skill_id { get; set; }

    public int years_experience { get; set; }

    public int skill_id { get; set; }

    public string candidate_id { get; set; } = null!;

    public virtual Candidate candidate { get; set; } = null!;

    public virtual Skill skill { get; set; } = null!;
}
