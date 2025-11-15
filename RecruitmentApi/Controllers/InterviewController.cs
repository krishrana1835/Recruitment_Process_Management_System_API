using Microsoft.AspNetCore.Mvc;
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
        public IActionResult ScheduleInterviews([FromBody] SheduleInterviewService.ScheduleInterviewRequestDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request body." });

            if (request.candidates == null || request.candidates.Count == 0)
                return BadRequest(new { message = "Candidates list cannot be empty." });

            if (request.interviewers == null || request.interviewers.Count == 0)
                return BadRequest(new { message = "Interviewers list cannot be empty." });

            try
            {
                var result = _scheduleService.ScheduleInterviews(request);
                return Ok(new
                {
                    success = true,
                    message = "Interviews scheduled successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while scheduling interviews.",
                    error = ex.Message
                });
            }
        }
    }
}
