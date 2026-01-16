using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Interview_FeedbackController : ControllerBase
    {
        private readonly InterviewFeedbackService _service;
        public Interview_FeedbackController(InterviewFeedbackService service)
        {
            _service = service;
        }

        [HttpPost("AddOrUpdateFeedback")]
        [Authorize(Roles = "Admin, HR, Interviewer")]
        public async Task<IActionResult> addOrUpdaeteFeedback([FromBody]Interview_FeedBackDtos.InterviewSkillSubmissionDto req)
        {
            try
            {
                await _service.AddOrUpdateCandidateFeedback(req);
                return Ok(new
                {
                    success = true,
                    message = "Candidate interview feedback added successfully",
                    data = true
                });
            }
            catch(NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = true,
                    message = "An error occured during inserting interview feedback",
                    error = ex.Message
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500,new
                {
                    success = true,
                    message = "An error occured during inserting interview feedback",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetFeedback")]
        [Authorize(Roles = "Admin, HR, Interviewer")]
        public async Task<IActionResult> getFeedback([FromQuery] int interview_id,[FromQuery] string user_id)
        {
            try
            {
                var res = await _service.GetCandidateFeedbackForInterview(interview_id, user_id);
                return Ok(new
                {
                    success = true,
                    message = "Candidate interview feedback added successfully",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = true,
                    message = "An error occured during inserting interview feedback",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = true,
                    message = "An error occured during inserting interview feedback",
                    error = ex.Message
                });
            }
        }
    }
}
