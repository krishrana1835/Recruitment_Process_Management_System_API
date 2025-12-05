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
        public async  Task<IActionResult> ScheduleInterviews([FromBody] SheduleInterviewService.ScheduleInterviewRequestDto request)
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
            catch(NullReferenceException ex)
            {
                return NotFound(new { success = false, message = "An error occurred while scheduling interviews." , error = ex.Message});
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
    }
}
