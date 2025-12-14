namespace RecruitmentApi.Dtos
{
    public class Jobs_SkillsDtos
    {
        public class Jobs_SkillDto
        {
            public int job_id { get; set; }
            public int skill_id { get; set; }
            public string skill_type { get; set; } = null!;
        }

        public class InterviewJobSkillRes
        {
            public int job_id { get; set; }
            public int skill_id { get; set; }
            public string skill_type { get; set; } = null!;
            public SkillDtos.SkillDto skill { get; set; }
        }
    }
}
