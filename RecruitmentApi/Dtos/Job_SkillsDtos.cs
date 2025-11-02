namespace RecruitmentApi.Dtos
{
    public class Jobs_SkillsDtos
    {
        public class AddJobs_SkillDto
        {
            public int job_id { get; set; }
            public int skill_ids { get; set; }
            public string skill_type { get; set; } = null!;
        }
    }
}
