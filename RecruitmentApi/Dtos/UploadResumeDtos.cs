using System.ComponentModel.DataAnnotations;

namespace RecruitmentApi.Dtos
{
    public class UploadResumeDtos
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }

    public class UploadCandidateDocuments
    {
        public string foldername { get; set; } = null!;
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
