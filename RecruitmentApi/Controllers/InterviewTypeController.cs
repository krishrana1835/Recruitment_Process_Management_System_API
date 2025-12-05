using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewTypeController : ControllerBase
    {
        private readonly Interview_TypeService _service;
        public InterviewTypeController(Interview_TypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> getAllInterviewTypes()
        {
            try
            {
                var response = await _service.FetchAllInterviewTypes();
                return Ok(new
                {
                    success = true,
                    message = "All Interview Type Fetched Successfully",
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest( new {
                    success = false,
                    message = ex.Message,
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> AddInterviewType([FromBody] Interview_TypeDtos.AddInterviewType dto)
        {
            try
            {
                var response = await _service.AddNewInterviewType(dto);
                return Ok(new
                {
                    success = true,
                    message = "Interview Type added Successfully",
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> UpdateInterviewType([FromBody] Interview_TypeDtos.InterviewType dto)
        {
            try
            {
                var response = await _service.UpdateInterviewType(dto);
                return Ok(new
                {
                    success = true,
                    message = "Interview Type Updated Successfully",
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Recruiter")]
        public async Task<IActionResult> deleteInterviewTypes([FromBody] Interview_TypeDtos.DeleteInterviewType dto)
        {
            try
            {
                var response = await _service.DeleteInterviewType(dto);
                return Ok(new
                {
                    success = true,
                    message = "Interview Type deleted Successfully",
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                });
            }
        }
    }
}
