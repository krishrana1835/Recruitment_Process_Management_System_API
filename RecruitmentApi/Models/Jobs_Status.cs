using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Jobs_Status
{
    public int status_id { get; set; }

    public string status { get; set; } = null!;

    public string? reason { get; set; }

    public DateTime changed_at { get; set; }

    public string changed_by { get; set; } = null!;

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual User changed_by_user { get; set; } = null!;
}
