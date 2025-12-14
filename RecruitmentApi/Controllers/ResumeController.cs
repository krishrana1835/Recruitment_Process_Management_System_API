// Controllers/ResumeController.cs

using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeParserService _parserService;

        public ResumeController(IResumeParserService parserService)
        {
            _parserService = parserService;
        }

        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AnalyzeResume([FromForm] UploadResumeDtos file)
        {
            try
            {
                if (file?.File == null || file.File.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var result = await _parserService.AnalyzeResumeAsync(file.File);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        public class Path
        {
            public string path { get; set; } = null!;
        }

        [HttpPost("GetCandidateSkills")]
        public async Task<IActionResult> AnalyzeResume([FromBody]Path path)
        {
            try
            {
                var result = await _parserService.AnalyzeCandidateResumeInterviewAsync(path.path);
                return Ok(new
                {
                    success = true,
                    message = "Candidate Skills from resume successfully fetched",
                    data=result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    message = "An error occured during fetching candidate skills from resume",
                    error = ex.Message
                });
            }
        }

    }
}
