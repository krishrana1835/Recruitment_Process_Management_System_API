using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecruitmentApi.Models;

public partial class Candidate_Document
{
    public int DocumentId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string VerificationStatus { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public string CandidateId { get; set; } = null!;

    public virtual Candidate Candidate { get; set; } = null!;
}
