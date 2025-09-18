using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Candidate_Status_History
{
    public int candidate_status_id { get; set; }

    public string status { get; set; } = null!;

    public string? reason { get; set; }

    public DateTime changed_at { get; set; }

    public string candidate_id { get; set; } = null!;

    public int job_id { get; set; }

    public string changed_by { get; set; } = null!;

    public virtual Candidate candidate { get; set; } = null!;

    public virtual User changed_by_user { get; set; } = null!;

    public virtual Job job { get; set; } = null!;
}
