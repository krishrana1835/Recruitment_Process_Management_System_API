using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly SheduleInterviewService _scheduleService;

        public InterviewController(SheduleInterviewService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Schedule or reschedule interviews for a specific job and round.
        /// </summary>
        [HttpPost("schedule")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> ScheduleInterviews([FromBody] SheduleInterviewService.ScheduleInterviewRequestDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request body." });

            if (request.interviewers == null || request.interviewers.Count == 0)
                return BadRequest(new { message = "Interviewers list cannot be empty." });

            try
            {
                var result = await _scheduleService.ScheduleInterviews(request);
                return Ok(new
                {
                    success = true,
                    message = "Interviews scheduled successfully.",
                    data = result
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { success = false, message = "An error occurred while scheduling interviews.", error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while scheduling interviews.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("InterviewRounds")]
        [Authorize(Roles = "Admin, Recruiter, Interviewer, HR, Viewer")]
        public async Task<IActionResult> fetchRounds([FromQuery] int job_id)
        {
            try
            {
                var response = await _scheduleService.FetchInterviewRounds(job_id);
                return Ok(new
                {
                    success = true,
                    message = "Rounds fetch Successfully!",
                    data = response
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { success = false, message = "An error occurred while fetching rounds", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching rounds.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetAllInterviewers")]
        public async Task<IActionResult> GetInterviewers([FromQuery] int job_id)
        {
            try
            {
                var res = await _scheduleService.GetAllInterviewers(job_id);
                return Ok(new
                {
                    success = true,
                    message = "Interviewers fetched successfully",
                    data = res
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetchig interviewers",
                    error = ex.Message
                });
            }
        }

        [HttpGet("JobSkills")]
        [Authorize(Roles = "Admin, Interviewer")]
        public async Task<IActionResult> fetchJobSkills([FromQuery] int job_id)
        {
            try
            {
                var response = await _scheduleService.FetchJobSkills(job_id);
                return Ok(new
                {
                    success = true,
                    message = "Job Skills fetched Successfully!",
                    data = response
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new 
                {
                    success = false,
                    message = "An error occurred while fetching job skills",
                    error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching job skills.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetAllShorlistedCandidates")]
        [Authorize(Roles = "Admin, Reviewer, Recruiter, Viewer")]
        public async Task<IActionResult> getAllShortlistedCandidates([FromQuery] int job_id)
        {
            try
            {
                var res = await _scheduleService.GetAllShortlistedCandidates(job_id);
                return Ok(new
                {
                    success = true,
                    message = "Candidates fetched successfully",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occurred while fetching candidates",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching candidates",
                    error = ex.Message
                });
            }
        }

        [HttpPost("CandidateInterviewSchedule")]
        [Authorize(Roles = "Admin, Recruiter, Viewer")]
        public async Task<IActionResult> fetchCandidateShedule(InterviewDtos.ListCandidateSheduleReq req)
        {
            try
            {
                var res = await _scheduleService.FetchCandidateInterviewShedule(req);
                return Ok(new
                {
                    success = true,
                    message = "Candidate Shedule successfully fetched",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occurred while fetching candidate interview shcedule.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching candidate intreview schedule.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("CandidateInterviewSchedule/Interviewer")]
        [Authorize(Roles = "Admin, Interviewer, HR")]
        public async Task<IActionResult> fetchCandidateSheduleInterviewer(InterviewDtos.ListCandidateSheduleReq req)
        {
            try
            {
                var res = await _scheduleService.FetchCandidateInterviewSheduleByInterviewer(req);
                return Ok(new
                {
                    success = true,
                    message = "Candidate Shedule successfully fetched",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occurred while fetching candidate interview shcedule.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching candidate intreview schedule.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetSkillDataForInterview")]
        [Authorize(Roles = "Admin, Interviewer, HR")]
        public async Task<IActionResult> getSkillData([FromQuery] int interview_id)
        {
            try
            {
                var res = await _scheduleService.FetchSkillDataForInterview(interview_id);
                return Ok(new
                {
                    success = true,
                    message = "Skill data fetched successfully",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occurred while fetching skill data.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching skill data.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("CheckCandidateInterviewHistory")]
        [Authorize(Roles = "Admin, HR, Interviewer")]
        public async Task<IActionResult> checkCandidateInterviewHistory([FromQuery]int interview_id)
        {
            try
            {
                var res = await _scheduleService.CheckCandidateInterviewHistory(interview_id);
                return Ok(new
                {
                    success = true,
                    message = "Interview history fetched successflly.",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured during fetcing candidate status history",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured during fetcing candidate status history",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetSelectedCandidates")]
        [Authorize(Roles = "Admin, HR, Viewer")]
        public async Task<IActionResult> getSelectedCandidates([FromQuery] int job_id)
        {
            try
            {
                var res = await _scheduleService.FetchSelectedCandidates(job_id);
                return Ok(new
                {
                    success = true,
                    message = "Selected candidates fetched successflly.",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured during fetcing candidate status history",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured during fetcing candidate status history",
                    error = ex.Message
                });
            }
        }

        [HttpGet("CandidateInterview")]
        [Authorize(Roles = "Admin, Recruiter, Candidate")]
        public async Task<IActionResult> fetchCandidateInterviews([FromQuery] int job_id, [FromQuery] string candidate_id)
        {
            try
            {
                var res = await _scheduleService.FetchCandidateInterview(job_id, candidate_id);
                return Ok(new
                {
                    success = true,
                    message = "Candidate Interview Schedule fetched successfully.",
                    data = res
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occurred while fetching candidate interview shcedule.",
                    error = ex.Message
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occurred while fetching candidate interview shcedule.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching candidate intreview schedule.",
                    error = ex.Message
                });
            }

        }

        [HttpPut("UpdateInterviewStatus")]
        [Authorize(Roles = "Admin, HR, Interviewer")]
        public async Task<IActionResult> updateCandidateStatus([FromBody] InterviewDtos.InterviewStatusUpdateReq req)
        {
            try
            {
                if (req.status.IsNullOrEmpty())
                    throw new ArgumentException("Invalid input provided");

                await _scheduleService.UpdateCandidateInterviewStatus(req.interview_id, req.status, req.user_id);
                return Ok(new
                {
                    success = true,
                    message = "Interview status successfully updated",
                    data = true
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid input provided",
                    error = ex.Message
                });
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured during updating status",
                    error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid input provided",
                    error = ex.Message
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while updating interview status",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("DeleteCandidateInterview")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> deleteCandiateInterview([FromQuery]int interview_id)
        {
            try
            {
                var res = await _scheduleService.DeleteCandidateInterview(interview_id);
                return Ok(new
                {
                    success = true,
                    message = "Candidate Interview Deleted Successfully!",
                    data = res
                });
            }
            catch(NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occurred while deleting candidate interview.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while deleting candidate intreview.",
                    error = ex.Message
                });
            }
        }

        [HttpPut("UpdateCandidateSchedule")]
        [Authorize(Roles = "Admin, Recruiter, Interviewer, HR")]
        public async Task<IActionResult> updateCandidateSchedule(InterviewDtos.UpdateCandidateScheduleReq req)
        {
            try
            {
                var res = await _scheduleService.UpdateCandidateSchedule(req);
                return Ok(new
                {
                    success = true,
                    message = "Candidate Schedule updated successfully!",
                    data = res
                });
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured while updating caniddate schedule.",
                    error = ex.Message
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured while updating caniddate schedule.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while updating caniddate schedule.",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("InterviewRound")]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> deleteRounds(InterviewDtos.DeleteSheduleReq req)
        {
            try
            {
                var response = await _scheduleService.DeleteInterviewSchedule(req);
                return Ok(new
                {
                    success = true,
                    message = "Round Deleted Successfully!",
                    data = response
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new { success = false, message = "An error occurred while deleting.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while deleting round",
                    error = ex.Message
                });
            }
        }
    }
}
