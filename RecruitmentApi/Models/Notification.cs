using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string Message { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
