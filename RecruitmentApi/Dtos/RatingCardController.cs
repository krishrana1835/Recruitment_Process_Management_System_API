using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Services;
using static RecruitmentApi.Services.RatingCardService;

namespace RecruitmentApi.Dtos
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingCardController : ControllerBase
    {
        private RatingCardService _service;

        public RatingCardController(RatingCardService service)
        {
            _service = service;
        }

        [HttpPost("GetRoundRating")]
        [Authorize(Roles = "Admin, Interviewer, HR")]
        public async Task<IActionResult> getRoundRating(RoundCardReq req)
        {
            try
            {
                var res = await _service.GetRatingByRoundAsync(req);
                return Ok(new
                {
                    success =true,
                    message = "Round rating fetched successfully.",
                    data=res
                });
            }
            catch(NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured while fetching rating",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new
                {
                    success = false,
                    message = "An error occured while fetching rating",
                    error = ex.Message
                });
            }
        }
    }
}
