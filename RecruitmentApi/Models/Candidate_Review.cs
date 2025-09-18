using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Candidate_Review
{
    public int review_id { get; set; }

    public string comments { get; set; } = null!;

    public DateTime reviewed_at { get; set; }

    public string candidate_id { get; set; } = null!;

    public int job_id { get; set; }

    public string user_id { get; set; } = null!;

    public virtual Candidate candidate { get; set; } = null!;

    public virtual Job job { get; set; } = null!;

    public virtual User user { get; set; } = null!;
}
