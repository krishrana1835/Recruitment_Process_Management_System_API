using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Jobs_Status
{
    public int StatusId { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public DateTime ChangedAt { get; set; }

    public string ChangedBy { get; set; } = null!;

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual User ChangedByUser { get; set; } = null!;
}
