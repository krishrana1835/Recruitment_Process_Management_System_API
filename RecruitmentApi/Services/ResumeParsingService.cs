using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentApi.Data;
using System.Diagnostics;
using System.Text.Json;

namespace RecruitmentApi.Services
{
    public interface IResumeParserService
    {
        Task<object> AnalyzeResumeAsync(IFormFile file);
        Task<object> AnalyzeCandidateResumeInterviewAsync(string filePath);
    }

    /// <summary>
    /// Service for analyzing resumes and extracting candidate details.
    /// </summary>
    public class ResumeParserService : IResumeParserService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Constructor to initialize ResumeParserService.
        /// </summary>
        public ResumeParserService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Analyzes a resume file and returns extracted information.
        /// </summary>
        /// <param name="file">Uploaded resume (PDF or DOCX).</param>
        /// <returns>Parsed data (name, email, phone, skills).</returns>
        public async Task<object> AnalyzeResumeAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Resume file is empty or missing.");

            // Save resume temporarily
            var tempPath = Path.GetTempFileName();
            var ext = Path.GetExtension(file.FileName);
            var resumePath = Path.ChangeExtension(tempPath, ext);

            await using (var stream = new FileStream(resumePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                //Fetch skill list from DB
                var skills = await _context.Skills.Select(s => s.SkillName).ToListAsync();
                var skillsCsv = string.Join(",", skills);

                //Correct Python script path (Services\Python)
                var scriptPath = Path.Combine(_env.ContentRootPath, "Services", "Python", "resume_parser.py");

                //Optional: Check if file exists to throw meaningful error early
                if (!System.IO.File.Exists(scriptPath))
                    throw new FileNotFoundException($"Python script not found at path: {scriptPath}");

                //Start Python process
                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{scriptPath}\" \"{resumePath}\" --skills \"{skillsCsv}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                var output = await process.StandardOutput.ReadToEndAsync();
                var errors = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new Exception("Python script error: " + errors);

                var result = JsonSerializer.Deserialize<object>(output);
                return result ?? new { message = "No result returned from Python script" };
            }
            catch (Exception ex)
            {
                throw new Exception("Resume parsing failed: " + ex.Message);
            }
            finally
            {
                //Clean up temp file
                if (System.IO.File.Exists(resumePath))
                    System.IO.File.Delete(resumePath);
            }
        }

        public async Task<object> AnalyzeCandidateResumeInterviewAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Resume path is empty.");

            // Convert relative path (/uploads/resumes/...) to actual file in wwwroot
            var resumePath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));

            if (!System.IO.File.Exists(resumePath))
                throw new FileNotFoundException("Resume file not found: " + resumePath);

            try
            {
                // Load skills from DB
                var skillEntities = await _context.Skills
                    .Select(s => new { s.SkillId, s.SkillName })
                    .ToListAsync();

                var allSkillsCsv = string.Join(",", skillEntities.Select(s => s.SkillName));

                var scriptPath = Path.Combine(
                    _env.ContentRootPath,
                    "Services",
                    "Python",
                    "resume_parser.py"
                );

                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{scriptPath}\" \"{resumePath}\" --skills \"{allSkillsCsv}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                var output = await process.StandardOutput.ReadToEndAsync();
                var errors = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new Exception(errors);

                // Deserialize python result (raw)
                var parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(output);

                if (parsed == null || !parsed.ContainsKey("skills"))
                    throw new Exception("Invalid output from resume parser.");

                // Extract detected skills
                var detectedSkillNames = JsonSerializer.Deserialize<List<string>>(parsed["skills"].ToString());

                // Map to DB skill IDs
                var matchedSkills = skillEntities
                    .Where(db => detectedSkillNames.Contains(db.SkillName, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                // Build final response
                var finalResult = new
                {
                    skills = matchedSkills
                };

                return finalResult;
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = "Resume parsing failed",
                    error = ex.Message
                };
            }
        }
    }
}
