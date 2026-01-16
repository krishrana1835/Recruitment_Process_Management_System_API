using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Candidate_Review> CandidateReviews { get; set; } = new List<Candidate_Review>();

    public virtual ICollection<Candidate_Status_History> CandidateStatusHistories { get; set; } = new List<Candidate_Status_History>();

    public virtual ICollection<Interview_Feedback> InterviewFeedbacks { get; set; } = new List<Interview_Feedback>();

    public virtual ICollection<Interview> AttendedInterviews { get; set; } = new List<Interview>();

    public virtual ICollection<Interview> ScheduledInterviews { get; set; } = new List<Interview>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Jobs_Status> JobsStatuses { get; set; } = new List<Jobs_Status>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<HR_Review> HrReviews { get; set; } = new List<HR_Review>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
