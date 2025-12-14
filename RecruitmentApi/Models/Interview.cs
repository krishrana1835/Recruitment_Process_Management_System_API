using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models;

public partial class Interview
{
    [Key]
    [Column("interview_id")]
    public int interview_id { get; set; }

    [Required]
    [Column("round_number")]
    public int round_number { get; set; }

    [Required]
    [Column("location_or_link")]
    public string location_or_link { get; set; } = null!;

    [Required]
    [Column("candidate_id")]
    public string candidate_id { get; set; } = null!;

    [Required]
    [Column("job_id")]
    public int job_id { get; set; }

    [Required]
    [Column("scheduled_by")]
    public string scheduled_by { get; set; } = null!;

    [Required]
    [Column("interview_type_id")]
    public int interview_type_id { get; set; }

    [Required]
    [Column("mode")]
    public string mode { get; set; } = null!;

    [Required]
    [Column("start_time")]
    public DateTime start_time { get; set; }

    [Required]
    [Column("end_time")]
    public DateTime end_time { get; set; }

    [Required]
    [Column("status")]
    public string status { get; set; } = null!;

    // Foreign Keys
    [ForeignKey("candidate_id")]
    public virtual Candidate candidate { get; set; } = null!;

    [ForeignKey("interview_type_id")]
    public virtual Interview_Type interview_type { get; set; } = null!;

    [ForeignKey("job_id")]
    public virtual Job job { get; set; } = null!;

    [ForeignKey("scheduled_by")]
    public virtual User scheduled_by_user { get; set; } = null!;

    public virtual ICollection<Interview_Feedback> Interview_Feedbacks { get; set; } = new List<Interview_Feedback>();

    public virtual ICollection<HR_Review> HR_Reviews { get; set; } = new List<HR_Review>();

    public virtual ICollection<User> users { get; set; } = new List<User>();
}
