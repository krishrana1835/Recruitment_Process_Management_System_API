using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly JobService _jobService;
        public JobController(JobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Candidate,Reviewer, Recruiter")]
        public async Task<IActionResult> getAllJobs()
        {
            try
            {
                var jobs = await _jobService.GetAllJobsAsync();
                return Ok(jobs);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred while fetching jobs.",
                    details = ex.Message
                });
            }
        }

        [HttpGet("{job_id:int}")]
        [Authorize(Roles = "Admin,Candidate, Reviewer, Recruiter")]
        public async Task<IActionResult> getJob(int job_id)
        {
            try
            {
                var job = await _jobService.GetJobAsync(job_id);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occured while fetching job.",
                    details = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recruiter,Recruiter")]
        public async Task<IActionResult> createNewJob([FromBody] JobDtos.CreateJobDto dto)
        {
            try
            {
                var job = await _jobService.AddJobAsync(dto);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occured while creating job.",
                    details = ex.Message
                });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> UpdateJob([FromBody] JobDtos.UpdateJobDto dto)
        {
            try
            {
                var job = await _jobService.UpdateJobAsync(dto);
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occured while updating job.",
                    details = ex.Message
                });
            }
        }

        [HttpDelete("{job_id:int}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> DeleteJob(int job_id)
        {
            try
            {
                var job = await _jobService.DeleteJobAsync(job_id);
                return Ok(new
                {
                    message = "Jobs deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occured while deleting job.",
                    details = ex.Message
                });
            }
        }

    }
}
