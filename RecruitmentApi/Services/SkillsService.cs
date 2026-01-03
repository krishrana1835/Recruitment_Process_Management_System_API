using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                skill_id = r.SkillId,
                skill_name = r.SkillName,
            }).ToList();
            return skillsdata;
        }

        public async Task<List<SkillDtos.SkillDto>> addSkills(string skill)
        {
            if(skill.IsNullOrEmpty())
                throw new ArgumentException("Invalid input");

            var skills = await _context.Skills.AnyAsync(s => s.SkillName == skill);

            if (skills)
                throw new InvalidOperationException("Skill already exist");

            var data = new Skill
            {
                SkillName = skill,
            };

            await _context.Skills.AddAsync(data);
            await _context.SaveChangesAsync();

            var response = await _context.Skills.Select(r => new SkillDtos.SkillDto
            {
                skill_id = r.SkillId,
                skill_name = r.SkillName,
            }).ToListAsync();

            return response;

        }

        public async Task<SkillDtos.SkillDto> updateSkill(SkillDtos.SkillDto req)
        {
            if (req.skill_name.IsNullOrEmpty())
                throw new ArgumentException("Skill name is empty");

            var skill = await _context.Skills.FirstOrDefaultAsync(i => i.SkillId == req.skill_id);
            if(skill==null) throw new NullReferenceException("Invalid skill id");

            skill.SkillName = req.skill_name;
            await _context.SaveChangesAsync();

            return new SkillDtos.SkillDto
            {
                skill_id = skill.SkillId,
                skill_name = skill.SkillName,
            };
        }

        public async Task<Boolean> deleteSkill(int skill_id)
        {

            var skill = await _context.Skills.FirstOrDefaultAsync(i => i.SkillId == skill_id);
            if (skill == null) throw new NullReferenceException("Invalid skill id");

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return true;
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
