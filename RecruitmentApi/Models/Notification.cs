using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Notification
{
    public int notification_id { get; set; }

    public string message { get; set; } = null!;

    public string status { get; set; } = null!;

    public DateTime created_at { get; set; }

    public string user_id { get; set; } = null!;

    public virtual User user { get; set; } = null!;
}
