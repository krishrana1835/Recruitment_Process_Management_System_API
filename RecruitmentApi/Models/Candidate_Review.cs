using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Candidate_Review
{
    public int ReviewId { get; set; }

    public string Comments { get; set; } = null!;

    public DateTime ReviewedAt { get; set; }

    public string CandidateId { get; set; } = null!;

    public int JobId { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
