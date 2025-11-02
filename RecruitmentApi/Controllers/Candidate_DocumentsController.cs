using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

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
        [Authorize(Roles = "Candidate")]
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


    }
}
