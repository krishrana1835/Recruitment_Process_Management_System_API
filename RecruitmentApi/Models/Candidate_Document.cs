using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecruitmentApi.Models;

public partial class Candidate_Document
{
    public int document_id { get; set; }

    public string document_type { get; set; } = null!;

    public string file_path { get; set; } = null!;

    public string verification_status { get; set; } = null!;

    public DateTime uploaded_at { get; set; }

    public string candidate_id { get; set; } = null!;

    public virtual Candidate candidate { get; set; } = null!;
}
