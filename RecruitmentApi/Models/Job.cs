using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Job
{
    public int JobId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string JobDescription { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public int StatusId { get; set; }

    public string Scheduled { get; set; } = null!;

    public virtual ICollection<Candidate_Review> CandidateReviews { get; set; } = new List<Candidate_Review>();

    public virtual ICollection<Candidate_Status_History> CandidateStatusHistories { get; set; } = new List<Candidate_Status_History>();

    public virtual ICollection<Employee_Record> EmployeeRecords { get; set; } = new List<Employee_Record>();

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<Jobs_Skill> JobsSkills { get; set; } = new List<Jobs_Skill>();

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Jobs_Status Status { get; set; } = null!;
}
