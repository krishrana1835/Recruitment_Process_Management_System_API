using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models;

public partial class User
{
    public string user_id { get; set; } = null!;

    public string name { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public DateTime created_at { get; set; }

    public virtual ICollection<Candidate_Review> Candidate_Reviews { get; set; } = new List<Candidate_Review>();

    public virtual ICollection<Candidate_Status_History> Candidate_Status_Histories { get; set; } = new List<Candidate_Status_History>();

    public virtual ICollection<Interview_Feedback> Interview_Feedbacks { get; set; } = new List<Interview_Feedback>();

    public virtual ICollection<Interview> AttendedInterviews { get; set; } = new List<Interview>();

    public virtual ICollection<Interview> ScheduledInterviews { get; set; } = new List<Interview>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Jobs_Status> Jobs_Statuses { get; set; } = new List<Jobs_Status>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<HR_Review> HR_Reviews { get; set; } = new List<HR_Review>();

    public virtual ICollection<Role> roles { get; set; } = new List<Role>();
}
