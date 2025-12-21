using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Candidate_Status_HistoryController : ControllerBase
    {
        private readonly Candidate_Status_HistoryService _service;

        public Candidate_Status_HistoryController(Candidate_Status_HistoryService service)
        {
            _service = service;
        }

        [HttpGet("GetApplications/{candidate_id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> checkAppliedForJob(string candidate_id)
        {
            try
            {
                var response = await _service.GetJobapplicationStatus(candidate_id);
                return Ok(new { applications = response});
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetAppliedJobs/{candidate_id}")]
        [Authorize(Roles = "Candidate, Admin")]
        public async Task<IActionResult> getAppliedJobs(string candidate_id)
        {
            try
            {
                var res = await _service.GetAppliedJobs(candidate_id);
                return Ok(new
                {
                    success = true,
                    message = "Applied jobs by candidate fetched successfully",
                    data = res
                });
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured while fetching applied jobs",
                    error = ex.Message
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured while fetching applied jobs",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetching applied jobs",
                    error = ex.Message
                });
            }
        }

        [HttpPost("UpdateCandidateStatus")]
        [Authorize(Roles = "Admin, Reviewer")]
        public async Task<IActionResult> updateCandidateStatus([FromBody] Candidate_Status_HistoryDtos.UpdateCandidateStatusRequest dto)
        {
            try
            {
                var response = await _service.updateCandidateStatus(dto);
                return Ok(new { success = response });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("GetJobApplications")]
        [Authorize(Roles = "Reviewer, Admin, Viewer")]
        public async Task<IActionResult> getJobApplications([FromBody]Candidate_Status_HistoryDtos.JobapplicationRequest dto)
        {
            try
            {
                var response = await _service.GetJobApplications(dto.job_id);
                return Ok(response);
            }catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("CheckApplication")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> checkAppliedForJob([FromBody] Candidate_Status_HistoryDtos.JobApplicationByCandidate dto)
        {
            try
            {
                var response = await _service.CheckForApplicationAsync(dto);
                return Ok(new { applied = response });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> applyForJob([FromBody] Candidate_Status_HistoryDtos.JobApplicationByCandidate dto)
        {
            try
            {
                var response = await _service.ApplyForJobAsync(dto);
                return Ok(new { message = "Application registerd" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
