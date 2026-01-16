using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;
using static RecruitmentApi.Services.ReportServices;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportServices _summaryService;

        public ReportsController(ReportServices summaryService)
        {
            _summaryService = summaryService;
        }

        [HttpPost("interview-summary")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> GetInterviewSummary([FromBody] ReportsDtos.InterviewSummaryRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("UserId and Role are required.");
            }

            try
            {
                var result = await _summaryService.GetInterviewSummaryAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Result successfully fetched",
                    data = result
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetchig data",
                    error = ex.Message
                });
            }
        }

        [HttpPost("TechWiseProfiles")]
        public async Task<IActionResult> GetTechWiseProfiles(ReportsDtos.TechReq request)
        {
            try
            {
                var res = await _summaryService.GetTechWiseProfiles(request);
                return Ok(new
                {
                    success = true,
                    messaage = "Data fetched successfully",
                    data = res
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetchig data",
                    error = ex.Message
                });
            }
        }

        [HttpPost("ExperienceWiseProfiles")]
        public async Task<IActionResult> GetExperienceWiseReport(ReportsDtos.ExpirienceReq request)
        {
            try
            {
                var res = await _summaryService.GetExpirienceWiseProfiles(request);
                return Ok(new
                {
                    success = true,
                    messaage = "Data fetched successfully",
                    data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetchig data",
                    error = ex.Message
                });
            }
        }

        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummary([FromQuery] DateTime date)
        {
            try
            {
                var res = await _summaryService.GetDailySummaryAsync(date);
                return Ok(new {
                    success = true,
                    message = "Daily summary succefully generated",
                    data= res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while generating the report.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("candidate-summary")]
        public async Task<IActionResult> GetCandidateSummary([FromQuery] string CandidateId)
        {
            try
            {
                var res = await _summaryService.GetCandidateSummaryAsync(CandidateId);
                return Ok(new
                {
                    success = true,
                    message = "Candidate summary succefully generated",
                    data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while generating the report.",
                    error = ex.Message
                });
            }
        }
    }
}
