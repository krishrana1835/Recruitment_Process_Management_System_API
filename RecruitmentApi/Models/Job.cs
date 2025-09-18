using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Job
{
    public int job_id { get; set; }

    public string job_title { get; set; } = null!;

    public string job_description { get; set; } = null!;

    public DateTime created_at { get; set; }

    public string created_by { get; set; } = null!;

    public int status_id { get; set; }

    public virtual ICollection<Candidate_Review> Candidate_Reviews { get; set; } = new List<Candidate_Review>();

    public virtual ICollection<Candidate_Status_History> Candidate_Status_Histories { get; set; } = new List<Candidate_Status_History>();

    public virtual ICollection<Employee_Record> Employee_Records { get; set; } = new List<Employee_Record>();

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<Jobs_Skill> Jobs_Skills { get; set; } = new List<Jobs_Skill>();

    public virtual User created_by_user { get; set; } = null!;

    public virtual Jobs_Status status { get; set; } = null!;
}
