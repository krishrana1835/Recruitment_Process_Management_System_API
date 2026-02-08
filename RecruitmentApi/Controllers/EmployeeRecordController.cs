using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

namespace RecruitmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeRecordController : ControllerBase
    {
        private EmployeeRecordService _service;
        public EmployeeRecordController(EmployeeRecordService service) 
        {
            _service = service;
        }

        [HttpPost("AddEmployee")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> AddEmployee(Employee_RecordDtos.EmployeeInsertDto req)
        {
            try
            {
                await _service.AddEmployeeAsync(req);
                return Ok(new
                {
                    success = true,
                    message = "Employee Added successfully",
                    data = true
                });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured while adding employee",
                    error = ex.Message
                });
            }
            catch(NullReferenceException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured while adding employee",
                    error = ex.Message
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while adding employee",
                    error = ex.Message
                });
            }
        }

        [HttpGet("IsEmployee")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> IsCandidateEmployee([FromQuery]string CandidateId)
        {
            try
            {
                var res = await _service.IsEmployeeAsync(CandidateId);
                return Ok(new
                {
                    success = true,
                    message = "Employee status fetched successfully",
                    data = res
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An Error occured while checking candidate's employee statuss",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetOfferlatter")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> OfferLatter([FromQuery] string CandidateId)
        {
            try
            {
                var res = await _service.FetchOfferLatterAsync(CandidateId);
                return Ok(new
                {
                    success = true,
                    message = "Offer Latter and date fetched successfully",
                    data = res
                });
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message,
                    error = ex.Message,
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetching offer latter and date",
                    error = ex.Message,
                });
            }
        }

        [HttpGet("GetSelectedCandidates")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> GetSelectedCandidates([FromQuery]int job_id)
        {
            try
            {
                var res = await _service.FetchCandidatesForEmployee(job_id);
                return Ok(new
                {
                    success = true,
                    message = "Candidates successfully fetched",
                    data = res
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetching candidates",
                    error = ex.Message
                });
            }
        }

        [HttpGet("FetchEmployees")]
        [Authorize(Roles = "Admin, HR, Viewer, Recruiter")]
        public async Task<IActionResult> FetchEmployees([FromQuery] int job_id)
        {
            try
            {
                var res = await _service.FetchEmployeesAsync(job_id);
                return Ok(new
                {
                    success = true,
                    message = "Employees fetched successfully",
                    data = res
                });
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured while fetching employee",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while fetching employee",
                    error = ex.Message
                });
            }
        }

        [HttpPut("UpdateEmployee")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> UpdateEmployee(Employee_RecordDtos.EmployeeUpdateDto req)
        {
            try
            {
                await _service.UpdateEmployeeAsync(req);
                return Ok(new
                {
                    success = true,
                    message = "Employee updated successfully",
                    data = true
                });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured while updating employee",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while updating employee",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("DeleteEmployee")]
        [Authorize(Roles = "Admin, HR")]
        public async Task<IActionResult> DeleteEmployee([FromQuery] int EmployeeId)
        {
            try
            {
                await _service.DeleteEmployeeAsync(EmployeeId);
                return Ok(new
                {
                    success = true,
                    message = "Employee deleted successfully",
                    data = true
                });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "An error occured while deleting employee",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occured while deleting employee",
                    error = ex.Message
                });
            }
        }
    }
}
