using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    public class Candidate_SkillDtos
    {
        public class Candidate_SkillDto
        {
            public int candidate_skill_id { get; set; }

            public int years_experience { get; set; }

            public int skill_id { get; set; }

            public string candidate_id { get; set; } = null!;

            public virtual SkillDtos.SkillDto skill { get; set; } = null!;
        }
        public class AddCandidate_SkillDto
        {
            public string candidate_id { get; set; } = null!;
            public List<int> skill_ids { get; set; }
        }
    }
}
