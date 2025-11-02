using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class SkillsService
    {
        private AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public SkillsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SkillDtos.SkillDto?>> GetAllSkillsAsync()
        {
            var skills = await _context.Skills.ToListAsync();
            if (skills == null)
                throw new Exception("No Skills in database");
            var skillsdata = skills.Select(r => new SkillDtos.SkillDto
            {
                skill_id = r.skill_id,
                skill_name = r.skill_name,
            }).ToList();
            return skillsdata;
        }

        public async Task AddCandidateSkills(string candidate_id, int skill_id)
        {
            if (string.IsNullOrEmpty(candidate_id))
                throw new Exception("Candidate Id is null");

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(r => r.candidate_id == candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            var newSkill = new Candidate_Skill
            {
                candidate_id = candidate_id,
                skill_id = skill_id,
                years_experience = 0
            };

            await _context.Candidate_Skills.AddAsync(newSkill);
            await _context.SaveChangesAsync();
        }

        public async Task AddJobSkills(int job_id, int skill_id, string skilltype)
        {
            if (job_id <= 0)
                throw new Exception("Job Id is invalid");

            var job = await _context.Jobs
                .FirstOrDefaultAsync(r => r.job_id == job_id);

            if (job == null)
                throw new Exception("Job not found");

            var newSkill = new Jobs_Skill
            {
                job_id = job_id,
                skill_id = skill_id,
                skill_type = skilltype
            };

            await _context.Jobs_Skills.AddAsync(newSkill);
            await _context.SaveChangesAsync();
        }

    }
}
