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

        [HttpPost("GetFeedback")]
        [Authorize(Roles = "Admin, HR, Interviewer")]
        public async Task<IActionResult> getFeedback([FromBody] Interview_FeedBackDtos.GetFeedbackReq req)
        {
            try
            {
                var res = await _service.GetCandidateFeedbackForInterview(req.interview_id, req.user_id);
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
