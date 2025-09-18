using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Interview_Type
{
    public int interview_type_id { get; set; }

    public string interview_round_name { get; set; } = null!;

    public string? process_descreption { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
