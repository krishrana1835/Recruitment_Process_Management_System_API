using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models;

public partial class Employee_Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeId { get; set; }

    public DateOnly JoiningDate { get; set; }

    public string OfferLetterPath { get; set; } = null!;

    public string CandidateId { get; set; } = null!;

    public int JobId { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;
}
