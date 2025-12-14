using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;
using RecruitmentApi.Services;
using static RecruitmentApi.Dtos.Candidate_SkillDtos;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly SkillsService _skillsService;

        public SkillController(SkillsService skillsService)
        {
            _skillsService = skillsService;
        }
        [HttpGet]
        public async Task<ActionResult<List<SkillDtos.SkillDto>>> getSkills()
        {
            try
            {
                var skills = await _skillsService.GetAllSkillsAsync();
                return Ok(skills);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("AddSkill/{skill}")]
        [Authorize(Roles = "Admin, Recruiter, Interviewer")]
        public async Task<IActionResult> addSkills(string skill)
        {
            try
            {
                var res = await _skillsService.addSkills(skill);
                return Ok(new
                {
                    success = true,
                    message = "Skill added successfully!",
                    data = res
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured during adding new skill",
                    error = ex.Message
                });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured during adding new skill",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured during adding new skill",
                    error = ex.Message
                });
            }
        }

        [HttpPost("UpdateSkill")]
        [Authorize(Roles = "Admin, Recruiter, Interviewer")]
        public async Task<IActionResult> updateSkill(SkillDtos.SkillDto req)
        {
            try
            {
                var res = await _skillsService.updateSkill(req);
                return Ok(new
                {
                    success = true,
                    message = "Skill Updated Successfully!",
                    data = res
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured during updating new skill",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured during updating new skill",
                    error = ex.Message
                });
            }
        }


        [HttpDelete("DeleteSkill/{skill_id}")]
        [Authorize(Roles = "Admin, Recruiter, Interviewer")]
        public async Task<IActionResult> updateSkill(int skill_id)
        {
            try
            {
                var res = await _skillsService.deleteSkill(skill_id);
                return Ok(new
                {
                    success = true,
                    message = "Skill Deleted Successfully!",
                    data = res
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured during deleting new skill",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured during deleting new skill",
                    error = ex.Message
                });
            }
        }

        [HttpPost("AddCandidateSkills")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCandidateSkills([FromBody] Candidate_SkillDtos.AddCandidate_SkillDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.candidate_id) || dto.skill_ids == null || !dto.skill_ids.Any())
                return BadRequest("Invalid candidate ID or skill IDs");

            try
            {
                foreach (var skillId in dto.skill_ids)
                {
                    await _skillsService.AddCandidateSkills(dto.candidate_id, skillId);
                }
                return Ok(new { message = "Skills added", candidate_id = dto.candidate_id, skill_ids = dto.skill_ids });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
