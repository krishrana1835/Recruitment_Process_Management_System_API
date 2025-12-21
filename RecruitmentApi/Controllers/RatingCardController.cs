using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;
using static RecruitmentApi.Services.RatingCardService;

namespace RecruitmentApi.Controllers
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
        [Authorize(Roles = "Admin, Interviewer, HR, Viewer")]
        public async Task<IActionResult> getRoundRating(RatingCardDtos.RoundCardReq req)
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

        [HttpGet("GetCandidateAllScores/{job_id}/{round_number}/{candidate_id}")]
        [Authorize(Roles = "Admin, Interviewer, HR, Viewer")]
        public async Task<IActionResult> getCandidateScores(int job_id, int round_number, string candidate_id)
        {
            try
            {
                var res = await _service.GetRatingsAsync(job_id, round_number,candidate_id);
                return Ok(new
                {
                    success = true,
                    message = "Rating fetched successfully.",
                    data = res
                });
            }
            catch (NullReferenceException ex)
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
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetching rating",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetCandidateWithSocre/{job_id}/{round_number}")]
        [Authorize(Roles = "Admin, Interviewer, HR, Viewer")]
        public async Task<IActionResult> getCandidatesWithScore(int job_id, int round_number)
        {
            try
            {
                var res = await _service.GetCandidateWithScore(job_id, round_number);
                return Ok(new
                {
                    success = true,
                    message = "Round scores fetched successfully.",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured while fetching scores",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetching scores",
                    error = ex.Message
                });
            }
        }
    }
}
