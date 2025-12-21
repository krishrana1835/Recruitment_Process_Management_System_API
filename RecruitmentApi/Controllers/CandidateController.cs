using System.Security.Cryptography.Pkcs;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly CandidateService _candidateService;
        public CandidateController(CandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpGet("GetCandidateList")]
        [Authorize(Roles = "Admin,Recruiter, Viewer")]
        public async Task<IActionResult> getListOfCandidates()
        {
            try
            {
                var candidates = await _candidateService.GetAllCandidateListAsync();
                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetLastCandidateId")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> GetLastCandidateId()
        {
            try
            {
                var lastId = await _candidateService.GetLastCandidateIdAsync();
                return Ok(new { candidate_id = lastId });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }   
        }

        /// <summary>
        /// Gets the detailed profile of a specific candidate by their ID.
        /// </summary>
        /// <remarks>
        /// This endpoint is restricted to users with the 'Admin' role.
        /// </remarks>
        /// <param name="candidateId">The unique identifier of the candidate.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that represents the outcome of the operation.
        /// </returns>
        [HttpGet("GetCandidateProfile/{candidateId}")]
        [Authorize(Roles = "Admin, Reviewer, Recruiter, Viewer")]
        public async Task<IActionResult> GetCandidateProfile(string candidateId)
        {
            try
            {
                // Call the service to fetch the candidate profile data
                var candidateProfile = await _candidateService.GetCandidateProfileAsync(candidateId);
                return Ok(candidateProfile);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            
        }

        [HttpGet("GetCandidateDashboardProfile/{candidateId}")]
        [Authorize(Roles = "Candidate, Reviewer, Viewer")]
        public async Task<IActionResult> GetCandidateDashProfile(string candidateId)
        {
            try
            {
                // Call the service to fetch the candidate profile data
                var candidateProfile = await _candidateService.GetCandidateDashProfile(candidateId);
                return Ok(candidateProfile);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        [HttpGet("GetResume/{candidate_id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetCandidateResume(string candidate_id)
        {
            try
            {
                var resume_path = await _candidateService.GetCandidateResume(candidate_id);
                return Ok(new { message = "Candidate Resume Fetched", resume_path = resume_path, success = true });
            }catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("AddCandidate")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> CreateCandidate([FromBody] CandidateDtos.CreateCandidateDto dto)
        {
            try
            {
                var candidate = await _candidateService.CreateCandidateAsync(dto);
                return Ok(new { message = "Candidate created", candidate_id = candidate.candidate_id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterCandidate([FromBody] CandidateDtos.RegisterCandidate dto)
        {
            try
            {
                var candidate = await _candidateService.RegisterCandidateAsync(dto);
                return Ok(new { message = "Candidate created", candidate_id = candidate.candidate_id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("UploadResume")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> UploadResume([FromBody] CandidateDtos.UploadCandidateResume dto)
        {
            try
            {
                var candidate = await _candidateService.UploadCandidateResumeAsync(dto);
                return Ok(new { message = "Resume Uploaded", url = dto.resume_path });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("UpdateCandidateDashboardProfile")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> UpdateCandidateDashProfile([FromBody] CandidateDtos.CandidateDashboardProfile candidateId)
        {
            try
            {
                // Call the service to fetch the candidate profile data
                var candidateProfile = await _candidateService.UpdateCandidateDashProfile(candidateId);
                return Ok(candidateProfile);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        [HttpPut("ResetPassword")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> ResetPassword([FromBody] CandidateDtos.ResetPasswrod dto)
        {
            try
            {
                var result = await _candidateService.ResetPasswordAsync(dto);
                return Ok(new { message = "Candidate Password Updated", success = result});
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("AddBulkCandidate")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> AddBulkCandidate([FromBody] CandidateDtos.CreateCandidateDto[] dtos)
        {
            try
            {
                var inserted_candidate_email = await _candidateService.CreateCandidatesBulkAsync(dtos);
                if (inserted_candidate_email.Count == 0)
                {
                    return BadRequest("No candidates were inserted. Possible duplicates or invalid data.");
                }
                return Ok(new { message = "Candidates are created", candidate_list = inserted_candidate_email });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("UpdateBulkCandidate")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> UpdateBulkCandidate([FromBody] CandidateDtos.UpdateCandidateDto[] dtos)
        {
            try
            {
                var updated_candidates_email = await _candidateService.UpdateCandidatesBulkAsync(dtos);
                if (updated_candidates_email.Count == 0)
                {
                    return BadRequest("No candidates were updated. Possible invalid data.");
                }
                return Ok(new { message = "Candidates are updated", candidate_list = updated_candidates_email });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("DeleteBulkCandidate")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> DeleteBulkCandidate([FromBody] CandidateDtos.DeleteCandidateDto[] dtos)
        {
            try
            {
                var deleted_candidates_id = await _candidateService.DeleteCandidatesBulkAsync(dtos);
                if (deleted_candidates_id.Count == 0)
                {
                    return BadRequest("No candidates were deleted. Possible invalid data.");
                }
                return Ok(new { message = "Candidates are deleted", candidate_list = deleted_candidates_id });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("DeleteCandidate/{candidate_id}")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> DeleteUser(String candidate_id)
        {
            try
            {
                if (await _candidateService.DeleteCandidateAsync(candidate_id))
                    return Ok(new { message = candidate_id + " is deleted", candidate_id = candidate_id });
                else return BadRequest("Error encounterd");
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
