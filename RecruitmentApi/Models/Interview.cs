using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models;

public partial class Interview
{
    public int InterviewId { get; set; }
    public int RoundNumber { get; set; }
    public string LocationOrLink { get; set; } = null!;
    public string CandidateId { get; set; } = null!;
    public int JobId { get; set; }
    public string ScheduledBy { get; set; } = null!;
    public int InterviewTypeId { get; set; }
    public string Mode { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string AllowFeedback { get; set; } = null!;
    public float Score { get; set; }
    public string Status { get; set; } = null!;
    public virtual Candidate Candidate { get; set; } = null!;
    public virtual Interview_Type InterviewType { get; set; } = null!;
    public virtual Job Job { get; set; } = null!;
    public virtual User ScheduledByUser { get; set; } = null!;

    public virtual ICollection<Interview_Feedback> InterviewFeedbacks { get; set; } = new List<Interview_Feedback>();

    public virtual ICollection<HR_Review> HrReviews { get; set; } = new List<HR_Review>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
