using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApi.Dtos;
using RecruitmentApi.Services;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly FileUploadService _fileUploadService;

    public FileUploadController(FileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpPost("CandidateResume")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadResume([FromForm] UploadResumeDtos model)
    {
        try
        {
            var url = await _fileUploadService.UploadCandidateResumeAsync(model.File, "resumes");
            return Ok(new { success = true, url });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("CandidateAadhar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAadhar([FromForm] UploadCandidateDocuments model)
    {
        try
        {
            var url = await _fileUploadService.UploadCandidateResumeAsync(model.File, "documents/" + model.foldername);
            return Ok(new { success = true, url });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("CandidatePan")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPan([FromForm] UploadCandidateDocuments model)
    {
        try
        {
            var url = await _fileUploadService.UploadCandidateResumeAsync(model.File, "documents/" + model.foldername);
            return Ok(new { success = true, url });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("CandidateCollegeResults")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPCollegeResults([FromForm] UploadCandidateDocuments model)
    {
        try
        {
            var url = await _fileUploadService.UploadCandidateResumeAsync(model.File, "documents/" + model.foldername);
            return Ok(new { success = true, url });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    
    [HttpPost("CandidateHscOrSscResult")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadHscOrSscResult([FromForm] UploadCandidateDocuments model)
    {
        try
        {
            var url = await _fileUploadService.UploadCandidateResumeAsync(model.File, "documents/"+model.foldername);
            return Ok(new { success = true, url });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete("CandidateResume")]
    [Authorize(Roles = "Admin, Candidate")]
    public async Task<IActionResult> DeleteResume([FromBody] DeleteResumeFile dto)
    {
        try
        {
            var url = await _fileUploadService.RemoveFileAsync(dto.resume_path);
            return Ok(new { success = true});
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete("RemoveDocument")]
    [Authorize(Roles = "Admin, Candidate")]
    public async Task<IActionResult> Deletedocument([FromBody] DeleteResumeFile dto)
    {
        try
        {
            var url = await _fileUploadService.RemoveFileAsync(dto.resume_path);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    public class DeleteResumeFile
    {
        public string resume_path { get; set; } = null!;
    }
}
