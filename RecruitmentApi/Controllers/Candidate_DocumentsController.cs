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
    public class Candidate_DocumentsController : ControllerBase
    {
        public readonly Candidate_DocumentService _service;

        public Candidate_DocumentsController(Candidate_DocumentService service)
        {
            this._service = service;
        }


        [HttpGet("{candidate_id}")]
        [Authorize(Roles = "Candidate, HR, Admin, Viewer")]
        public async Task<IActionResult> getCandidateDocuments(string candidate_id)
        {
            try
            {
                var response = await _service.GetCandidateDocumentsAsync(candidate_id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> addCandidateDocument([FromBody]Candidate_DocumentDtos.UploadCandidateDocuments dto)
        {
            try
            {
                var response = await _service.AddCandidateDocumentAsync(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("UpdateUploadStatus")]
        [Authorize(Roles = "Admin, HR, Admin")]
        public async Task<IActionResult> getUploadStatus(Candidate_DocumentDtos.ChangeStatusReq req)
        {
            try
            {
                var res = await _service.UpdateDocUploadStatus(req);
                return Ok(new
                {
                    success = true,
                    message = "Status fetched successfully",
                    data = res
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request",
                    error = ex.Message
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occoured during fetching status",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occoured during fetching status",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetUploadStatus/{candidate_id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> getUploadStatus(string candidate_id)
        {
            try
            {
                var res = await _service.FetchDocUploadStatus(candidate_id);
                return Ok(new
                {
                    success = true,
                    message = "Status fetched successfully",
                    data = res
                });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request",
                    error = ex.Message
                });
            }
            catch (NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occoured during fetching status",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new
                {
                    success = false,
                    message = "An error occoured during fetching status",
                    error = ex.Message
                });
            }
        }

        [HttpPost("UpdateVerificationStatus")]
        [Authorize(Roles = "Admin, HR, Admin")]
        public async Task<IActionResult> updateVerification(Candidate_DocumentDtos.VerificationStatus req)
        {
            try
            {
                var res = await _service.UpdateVerificationStatus(req);
                return Ok(new
                {
                    success = true,
                    message = "Status updated successfuoly",
                    data = res
                });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid status",
                    error = ex.Message
                });
            }
            catch(NullReferenceException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = "An error occured during updating status",
                    error = ex.Message
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500,new
                {
                    success = false,
                    message = "An error occured during updating status",
                    error = ex.Message
                });
            }
        }
    }
}
