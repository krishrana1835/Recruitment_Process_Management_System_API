using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models;

public partial class Candidate
{
    public string CandidateId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string ResumePath { get; set; } = null!;

    public Boolean DocUpload { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Candidate_Document> CandidateDocuments { get; set; } = new List<Candidate_Document>();

    public virtual ICollection<Candidate_Review> CandidateReviews { get; set; } = new List<Candidate_Review>();

    public virtual ICollection<Candidate_Skill> CandidateSkills { get; set; } = new List<Candidate_Skill>();

    public virtual ICollection<Candidate_Status_History> CandidateStatusHistories { get; set; } = new List<Candidate_Status_History>();

    public virtual Employee_Record? EmployeeRecord { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
