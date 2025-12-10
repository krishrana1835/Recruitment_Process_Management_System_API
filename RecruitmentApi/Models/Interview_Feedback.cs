using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Interview_Feedback
{
    public int feedback_id { get; set; }

    public int concept_rating { get; set; }

    public int technical_rating { get; set; }

    public string comments { get; set; } = null!;

    public DateTime feedback_at { get; set; }

    public int interview_id { get; set; }

    public string user_id { get; set; } = null!;

    public int candidate_skill_id { get; set; }

    public virtual Interview interview { get; set; } = null!;

    public virtual Candidate_Skill candidate_skill { get; set; } = null!;

    public virtual User user { get; set; } = null!;
}
