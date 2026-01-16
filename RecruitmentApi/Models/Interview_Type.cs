using System;
using System.Collections.Generic;

namespace RecruitmentApi.Models;

public partial class Interview_Type
{
    public int InterviewTypeId { get; set; }

    public string InterviewRoundName { get; set; } = null!;

    public string? ProcessDescreption { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
