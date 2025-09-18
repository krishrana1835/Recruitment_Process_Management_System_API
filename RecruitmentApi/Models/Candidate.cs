using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Candidate
{
    public string candidate_id { get; set; } = null!;

    public string full_name { get; set; } = null!;

    public string email { get; set; } = null!;

    public string? phone { get; set; }

    public string resume_path { get; set; } = null!;

    public DateTime created_at { get; set; }

    public virtual ICollection<Candidate_Document> Candidate_Documents { get; set; } = new List<Candidate_Document>();

    public virtual ICollection<Candidate_Review> Candidate_Reviews { get; set; } = new List<Candidate_Review>();

    public virtual ICollection<Candidate_Skill> Candidate_Skills { get; set; } = new List<Candidate_Skill>();

    public virtual ICollection<Candidate_Status_History> Candidate_Status_Histories { get; set; } = new List<Candidate_Status_History>();

    public virtual Employee_Record? Employee_Record { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
