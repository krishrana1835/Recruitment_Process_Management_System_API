using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HR_ReviewController : ControllerBase
    {
        private HrReviewService _service;

        public HR_ReviewController(HrReviewService service)
        {
            _service = service;
        }

        [HttpGet("GetReview/{interview_id}/{user_id}")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> getHrReview(int interview_id, string user_id)
        {
            try
            {
                var res = await _service.getHrReview(interview_id, user_id);
                return Ok(new
                {
                    sucess = true,
                    messaeg = "Review fetched successfully",
                    data = res
                });
            }
            catch(NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured while fetching review",
                    error = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetching review",
                    error = ex.Message,
                });
            }
        }

        [HttpPost("Add-Update")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> addOrUpdateReview([FromBody] HrReviewDtos.ReviewDto req)
        {
            try
            {
                var res = await _service.AddOrUpdateHrReview(req);
                return Ok(new
                {
                    sucess = true,
                    messaeg = "Review added or updated successfully",
                    data = res
                });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid input to add or update the review",
                    error = ex.Message,
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500,new
                {
                    success = false,
                    message = "An error occured during adding or updating review",
                    error = ex.Message,
                });
            }
        }
    }
}
