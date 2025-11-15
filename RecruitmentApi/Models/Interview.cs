using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Interview
{
    public int interview_id { get; set; }

    public int round_number { get; set; }

    public string location_or_link { get; set; } = null!;

    public string candidate_id { get; set; } = null!;

    public int job_id { get; set; }

    public string scheduled_by { get; set; } = null!;

    public int interview_type_id { get; set; }

    public string mode { get; set; } = null!;

    public virtual Candidate candidate { get; set; } = null!;

    public virtual Interview_Type interview_type { get; set; } = null!;

    public virtual Job job { get; set; } = null!;

    public virtual User scheduled_by_user { get; set; } = null!;

    public virtual ICollection<Interview_Feedback> Interview_Feedbacks { get; set; } = new List<Interview_Feedback>();

    public virtual ICollection<User> users { get; set; } = new List<User>();
}
