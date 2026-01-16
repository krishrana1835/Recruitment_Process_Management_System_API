using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Candidate_Status_History
{
    public int CandidateStatusId { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public DateTime ChangedAt { get; set; }

    public string CandidateId { get; set; } = null!;

    public int JobId { get; set; }

    public string ChangedBy { get; set; } = null!;

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual User ChangedByUser { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;
}
