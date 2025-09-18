using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Employee_Record
{
    public string employee_id { get; set; } = null!;

    public DateOnly joining_date { get; set; }

    public string offer_letter_path { get; set; } = null!;

    public string candidate_id { get; set; } = null!;

    public int job_id { get; set; }

    public virtual Candidate candidate { get; set; } = null!;

    public virtual Job job { get; set; } = null!;
}
