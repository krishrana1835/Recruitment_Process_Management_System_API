using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Jobs_Skill
{
    public int skill_id { get; set; }

    public int job_id { get; set; }

    public string skill_type { get; set; } = null!;

    public virtual Job job { get; set; } = null!;

    public virtual Skill skill { get; set; } = null!;
}
