using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Jobs_Skill
{
    public int SkillId { get; set; }

    public int JobId { get; set; }

    public string SkillType { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
