using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetInterviewSummary([FromBody] InterviewSummaryRequest request)
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
        public async Task<IActionResult> GetTechWiseProfiles(TechReq request)
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
        public async Task<IActionResult> GetExperienceWiseReport(ExpirienceReq request)
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
    }
}
