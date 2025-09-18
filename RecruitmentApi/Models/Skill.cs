using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Skill
{
    public int skill_id { get; set; }

    public string skill_name { get; set; } = null!;

    public virtual ICollection<Candidate_Skill> Candidate_Skills { get; set; } = new List<Candidate_Skill>();

    public virtual ICollection<Interview_Feedback> Interview_Feedbacks { get; set; } = new List<Interview_Feedback>();

    public virtual ICollection<Jobs_Skill> Jobs_Skills { get; set; } = new List<Jobs_Skill>();
}
