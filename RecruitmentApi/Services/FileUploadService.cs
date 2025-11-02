using RecruitmentApi.Data;

namespace RecruitmentApi.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _defaultUploadPath;
        private readonly AppDbContext _context;

        public FileUploadService(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _defaultUploadPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");

            if (!Directory.Exists(_defaultUploadPath))
            {
                Directory.CreateDirectory(_defaultUploadPath);
            }

            _context = context;
        }

        // ✅ Uploads file to optional subdirectory
        public async Task<string> UploadCandidateResumeAsync(IFormFile file, string? subDirectory = null)
        {
            string uploadPath = _defaultUploadPath;

            if (!string.IsNullOrWhiteSpace(subDirectory))
            {
                uploadPath = Path.Combine(_defaultUploadPath, subDirectory);

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
            }

            return await UploadFileAsync(file, uploadPath, subDirectory);
        }

        private async Task<string> UploadFileAsync(IFormFile file, string uploadPath, string? subDirectory)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = !string.IsNullOrWhiteSpace(subDirectory)
                ? $"/uploads/{subDirectory}/{fileName}"
                : $"/uploads/{fileName}";

            return relativePath;
        }

        public Task<bool> RemoveFileAsync(string relativeFilePath)
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
                throw new ArgumentException("File path cannot be empty.");

            // Remove leading slash if present
            var cleanedPath = relativeFilePath.TrimStart('/');

            // Combine with wwwroot to get full path
            var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", cleanedPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            // File not found
            return Task.FromResult(false);
        }
    }
}
