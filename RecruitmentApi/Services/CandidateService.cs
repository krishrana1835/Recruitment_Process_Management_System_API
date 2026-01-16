using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;
using static Candidate_ReviewDtos;
using static RecruitmentApi.Dtos.Candidate_DocumentDtos;
using static RecruitmentApi.Dtos.Candidate_Status_HistoryDtos;
using static RecruitmentApi.Dtos.CandidateDtos;
using static RecruitmentApi.Dtos.Employee_RecordDtos;
using static RecruitmentApi.Dtos.InterviewDtos;

namespace RecruitmentApi.Services
{
    public class CandidateService
    {
        private AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public CandidateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CandidateDtos.CandidateListDto?>> GetAllCandidateListAsync()
        {
            var candidates = await _context.Candidates.Select(r => new CandidateDtos.CandidateListDto
            {
                candidate_id = r.CandidateId,
                full_name = r.FullName,
                email = r.Email,
                phone = r.Phone,
                created_at = r.CreatedAt,
            }).ToListAsync();

            if (candidates == null)
            {
                throw new Exception("No candidates found");
            }

            return candidates;
        }

        public async Task<string?> GetLastCandidateIdAsync()
        {
            var candidate = await _context.Candidates
                                .OrderByDescending(c => c.CreatedAt)
                                .FirstOrDefaultAsync();
            return candidate?.CandidateId;
        }

        /// <summary>
        /// Fetches a detailed profile for a specific candidate using their ID.
        /// This version is corrected to use the specific DTO structures provided.
        /// </summary>
        public async Task<CandidateDtos.CandidateProfileDto?> GetCandidateProfileAsync(string candidateId)
        {
            var candidateProfile = await _context.Candidates
                .AsNoTracking()
                .Where(c => c.CandidateId == candidateId)
                // Project directly into the main profile DTO
                .Select(c => new CandidateDtos.CandidateProfileDto
                {
                    // Base properties
                    candidate_id = c.CandidateId,
                    full_name = c.FullName,
                    email = c.Email,
                    phone = c.Phone,
                    resume_path = c.ResumePath,
                    created_at = c.CreatedAt,

                    // Map related collections to their specific nested DTOs
                    Candidate_Documents = c.CandidateDocuments.Select(doc => new Candidate_DocumentDtos.Candidate_DocumentDto
                    {
                        document_id = doc.DocumentId,
                        document_type = doc.DocumentType,
                        file_path = doc.FilePath,
                        verification_status = doc.VerificationStatus,
                        uploaded_at = doc.UploadedAt
                    }).ToList(),

                    Candidate_Reviews = c.CandidateReviews.Select(rev => new Candidate_ReviewDtos.CandidateReviewDto_Candidate
                    {
                        review_id = rev.ReviewId,
                        comments = rev.Comments,
                        reviewed_at = rev.ReviewedAt,
                        job_id = rev.JobId,
                        user_id = rev.UserId,
                        job = new JobDtos.JobDto_Candidate
                        {
                            job_id = rev.Job.JobId,
                            job_title = rev.Job.JobTitle
                        }
                    }).ToList(),

                    Candidate_Skills = c.CandidateSkills.Select(cs => new Candidate_SkillDtos.Candidate_SkillDto
                    {
                        candidate_skill_id = cs.CandidateSkillId,
                        years_experience = cs.YearsExperience,
                        skill_id = cs.SkillId,
                        candidate_id = cs.CandidateId,
                        // Map the related Skill entity to the SkillDto
                        skill = new SkillDtos.SkillDto
                        {
                            skill_id = cs.Skill.SkillId,
                            skill_name = cs.Skill.SkillName
                        }
                    }).ToList(),

                    Candidate_Status_Histories = c.CandidateStatusHistories.Select(hist => new Candidate_Status_HistoryDtos.Candidate_Status_HistoryDto_Candidate
                    {
                        candidate_status_id = hist.CandidateStatusId,
                        status = hist.Status,
                        reason = hist.Reason,
                        changed_at = hist.ChangedAt,
                        job = new JobDtos.JobTitle
                        {
                            job_title = hist.Job.JobTitle
                        }
                    }).ToList(),

                    Interviews = c.Interviews
                        .OrderBy(i => i.Job.JobTitle)
                        .ThenBy(i => i.RoundNumber)
                        .Select(i => new InterviewDtos.InterviewDtos_Candidate
                        {
                            interview_id = i.InterviewId,
                            job_id = i.JobId,
                            round_title = i.InterviewType.InterviewRoundName,
                            round_number = i.RoundNumber,
                            status = i.Status,
                            job = new JobDtos.JobDto_Candidate
                            {
                                job_id = i.Job.JobId,
                                job_title = i.Job.JobTitle
                            }
                        })
                        .ToList(),

                    Employee_Record = c.EmployeeRecord == null ? null : new Employee_RecordDtos.Employee_RecordDto_Candidate
                    {
                        employee_id = c.EmployeeRecord.EmployeeId,
                        joining_date = c.EmployeeRecord.JoiningDate,
                        offer_letter_path = c.EmployeeRecord.OfferLetterPath,
                        job_id = c.EmployeeRecord.JobId
                    }
                })
                .FirstOrDefaultAsync();

            if (candidateProfile == null)
                throw new Exception("Candidate Does not exist");

            return candidateProfile;
        }

        public async Task<string> GetCandidateResume(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate Id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            string resume_path = candidate.ResumePath;

            return resume_path;
        }

        public async Task<CandidateDtos.CandidateDashboardProfile> GetCandidateDashProfile(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            var response = new CandidateDtos.CandidateDashboardProfile
            {
                candidate_id = candidate.CandidateId,
                full_name = candidate.FullName,
                email = candidate.Email,
                phone = candidate.Phone
            };

            return response;
        }

        public async Task<CandidateDtos.CandidateDashboardProfile> UpdateCandidateDashProfile(CandidateDtos.CandidateDashboardProfile dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate id");

            if(dto.email.IsNullOrEmpty() || dto.phone.IsNullOrEmpty() || dto.full_name.IsNullOrEmpty())
                throw new ArgumentException("Invalid Input for fields");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            candidate.FullName = dto.full_name;
            candidate.Email = dto.email;
            candidate.Phone = dto.phone;

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<Boolean> IsRegisteredAsync(string email)
        {
            if (email.IsNullOrEmpty())
                throw new ArgumentException("Invalid Email");

            var data = await _context.Candidates.FirstOrDefaultAsync(r => r.Email == email);

            if (data != null)
                return true;
            return false;
        }

        public async Task<CandidateDtos.CreateCandidateDto> CreateCandidateAsync(CandidateDtos.CreateCandidateDto dto)
        {
            if (dto == null)
            {
                throw new Exception("Null values found");
            }

            if (string.IsNullOrWhiteSpace(dto.candidate_id) ||
                string.IsNullOrWhiteSpace(dto.full_name) ||
                string.IsNullOrWhiteSpace(dto.email) ||
                string.IsNullOrWhiteSpace(dto.resume_path) ||
                string.IsNullOrWhiteSpace(dto.password))
            {
                throw new Exception("One or more required fields are missing.");
            }

            var data = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id);

            if (data != null)
            {
                throw new Exception("Candidate already exist");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            Candidate candidate = new Candidate
            {
                CandidateId = dto.candidate_id,
                FullName = dto.full_name,
                Email = dto.email,
                Phone = dto.phone,
                ResumePath = dto.resume_path,
                CreatedAt = DateTime.Now,
                Password = hashedPassword,
            };

            await _context.Candidates.AddAsync(candidate);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<CandidateDtos.RegisterCandidate> RegisterCandidateAsync(CandidateDtos.RegisterCandidate dto)
        {
            if (dto == null)
            {
                throw new Exception("Null values found");
            }

            if (string.IsNullOrWhiteSpace(dto.candidate_id) ||
                string.IsNullOrWhiteSpace(dto.full_name) ||
                string.IsNullOrWhiteSpace(dto.email) ||
                string.IsNullOrWhiteSpace(dto.password))
            {
                throw new Exception("One or more required fields are missing.");
            }

            var data = await _context.Candidates.FirstOrDefaultAsync(r => r.Email == dto.email);

            if (data != null)
            {
                throw new Exception("Email already in use");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            Candidate candidate = new Candidate
            {
                CandidateId = dto.candidate_id,
                FullName = dto.full_name,
                Email = dto.email,
                Phone = dto.phone,
                ResumePath = "not provided",
                CreatedAt = DateTime.Now,
                Password = hashedPassword,
            };

            await _context.Candidates.AddAsync(candidate);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<Boolean> ResetPasswordAsync(CandidateDtos.ResetPasswrod dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid Candidate Id");
            if (dto.password.IsNullOrEmpty())
                throw new ArgumentException("Invalid Password format");
            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            candidate.Password = hashedPassword;

            await _context.SaveChangesAsync();
            return true;
        }  

        public async Task<CandidateDtos.UploadCandidateResume> UploadCandidateResumeAsync(CandidateDtos.UploadCandidateResume dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate Id");

            if (dto.resume_path.IsNullOrEmpty())
                throw new ArgumentException("Invalid Resume path");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            candidate.ResumePath = dto.resume_path;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<List<string>> CreateCandidatesBulkAsync(CandidateDtos.CreateCandidateDto[] dtos)
        {
            if (dtos.Length == 0)
                throw new Exception("No data available");

            List<string> insertedCandidateIds = new List<string>();

            foreach (var d in dtos)
            {
                if (string.IsNullOrWhiteSpace(d.candidate_id) ||
                    string.IsNullOrWhiteSpace(d.full_name) ||
                    string.IsNullOrWhiteSpace(d.email) ||
                    string.IsNullOrWhiteSpace(d.resume_path) ||
                    string.IsNullOrWhiteSpace(d.password))
                {
                    throw new Exception("Some fields are missing in data");
                }

                try
                {
                    var existingCandidate = await _context.Candidates.FirstOrDefaultAsync(r => r.Email == d.email);
                    if (existingCandidate != null)
                    {
                        throw new Exception($"Candidate with email {d.email} is already registered");
                    }

                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(d.password);

                    Candidate candidate = new Candidate
                    {
                        CandidateId = d.candidate_id,
                        FullName = d.full_name,
                        Email = d.email,
                        Phone = d.phone,
                        ResumePath = d.resume_path,
                        CreatedAt = DateTime.Now,
                        Password = hashedPassword,
                    };

                    await _context.Candidates.AddAsync(candidate);
                    await _context.SaveChangesAsync();

                    insertedCandidateIds.Add(candidate.Email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing candidate {d.email}: {ex.Message}");
                }
            }

            return insertedCandidateIds;
        }

        public async Task<List<string>> UpdateCandidatesBulkAsync(CandidateDtos.UpdateCandidateDto[] dtos)
        {
            if (dtos == null || dtos.Length == 0)
                throw new Exception("No data available");

            List<string> updatedCandidateEmails = new List<string>();

            foreach (var d in dtos)
            {
                if (string.IsNullOrWhiteSpace(d.candidate_id) ||
                    string.IsNullOrWhiteSpace(d.full_name) ||
                    string.IsNullOrWhiteSpace(d.email) ||
                    string.IsNullOrWhiteSpace(d.resume_path))
                {
                    // Password is optional for update
                    Console.WriteLine($"Skipping candidate {d.email} due to missing required fields.");
                    continue;
                }

                try
                {
                    var existingCandidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == d.candidate_id);

                    if (existingCandidate == null)
                    {
                        Console.WriteLine($"Candidate with ID {d.candidate_id} not found.");
                        continue;
                    }

                    // Update fields
                    existingCandidate.FullName = d.full_name;
                    existingCandidate.Email = d.email;
                    existingCandidate.Phone = d.phone;
                    existingCandidate.ResumePath = d.resume_path;

                    _context.Candidates.Update(existingCandidate);
                    await _context.SaveChangesAsync();

                    updatedCandidateEmails.Add(existingCandidate.Email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating candidate {d.email}: {ex.Message}");
                }
            }

            return updatedCandidateEmails;
        }

        public async Task<List<string>> DeleteCandidatesBulkAsync(CandidateDtos.DeleteCandidateDto[] dtos)
        {
            if (dtos == null || dtos.Length == 0)
                throw new Exception("No data available");

            List<string> deletedCandidateEmails = new List<string>();

            foreach (var d in dtos)
            {
                if (string.IsNullOrWhiteSpace(d.candidate_id))
                {
                    Console.WriteLine("Skipping candidate due to missing candidate_id.");
                    continue;
                }

                try
                {
                    var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.CandidateId == d.candidate_id);

                    if (candidate == null)
                    {
                        Console.WriteLine($"Candidate with ID {d.candidate_id} not found.");
                        continue;
                    }

                    _context.Candidates.Remove(candidate);
                    await _context.SaveChangesAsync();

                    deletedCandidateEmails.Add(candidate.CandidateId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting candidate with ID {d.candidate_id}: {ex.Message}");
                }
            }

            return deletedCandidateEmails;
        }


        public async Task<bool> DeleteCandidateAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Candidate ID cannot be null or empty.", nameof(id));

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == id);

            if (candidate == null)
                throw new KeyNotFoundException("Candidate not found with the specified ID.");

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
