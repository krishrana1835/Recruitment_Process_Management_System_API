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
                candidate_id = r.candidate_id,
                full_name = r.full_name,
                email = r.email,
                phone = r.phone,
                created_at = r.created_at,
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
                                .OrderByDescending(c => c.created_at)
                                .FirstOrDefaultAsync();
            return candidate?.candidate_id;
        }

        /// <summary>
        /// Fetches a detailed profile for a specific candidate using their ID.
        /// This version is corrected to use the specific DTO structures provided.
        /// </summary>
        public async Task<CandidateDtos.CandidateProfileDto?> GetCandidateProfileAsync(string candidateId)
        {
            var candidateProfile = await _context.Candidates
                .AsNoTracking()
                .Where(c => c.candidate_id == candidateId)
                // Project directly into the main profile DTO
                .Select(c => new CandidateDtos.CandidateProfileDto
                {
                    // Base properties
                    candidate_id = c.candidate_id,
                    full_name = c.full_name,
                    email = c.email,
                    phone = c.phone,
                    resume_path = c.resume_path,
                    created_at = c.created_at,

                    // Map related collections to their specific nested DTOs
                    Candidate_Documents = c.Candidate_Documents.Select(doc => new Candidate_DocumentDtos.Candidate_DocumentDto
                    {
                        document_id = doc.document_id,
                        document_type = doc.document_type,
                        file_path = doc.file_path,
                        verification_status = doc.verification_status,
                        uploaded_at = doc.uploaded_at
                    }).ToList(),

                    Candidate_Reviews = c.Candidate_Reviews.Select(rev => new Candidate_ReviewDtos.CandidateReviewDto_Candidate
                    {
                        review_id = rev.review_id,
                        comments = rev.comments,
                        reviewed_at = rev.reviewed_at,
                        job_id = rev.job_id,
                        user_id = rev.user_id,
                        job = new JobDtos.JobDto_Candidate
                        {
                            job_id = rev.job.job_id,
                            job_title = rev.job.job_title
                        }
                    }).ToList(),

                    Candidate_Skills = c.Candidate_Skills.Select(cs => new Candidate_SkillDtos.Candidate_SkillDto
                    {
                        candidate_skill_id = cs.candidate_skill_id,
                        years_experience = cs.years_experience,
                        skill_id = cs.skill_id,
                        candidate_id = cs.candidate_id,
                        // Map the related Skill entity to the SkillDto
                        skill = new SkillDtos.SkillDto
                        {
                            skill_id = cs.skill.SkillId,
                            skill_name = cs.skill.SkillName
                        }
                    }).ToList(),

                    Candidate_Status_Histories = c.Candidate_Status_Histories.Select(hist => new Candidate_Status_HistoryDtos.Candidate_Status_HistoryDto_Candidate
                    {
                        candidate_status_id = hist.candidate_status_id,
                        status = hist.status,
                        reason = hist.reason,
                        changed_at = hist.changed_at,
                        job = new JobDtos.JobTitle
                        {
                            job_title = hist.job.job_title
                        }
                    }).ToList(),

                    Interviews = c.Interviews.Select(i => new InterviewDtos.InterviewDtos_Candidate
                    {
                        interview_id = i.interview_id,
                        job_id = i.job_id,
                        job = new JobDtos.JobDto_Candidate
                        {
                            job_id = i.job.job_id,
                            job_title = i.job.job_title,
                        }
                    }).ToList(),

                    Employee_Record = c.Employee_Record == null ? null : new Employee_RecordDtos.Employee_RecordDto_Candidate
                    {
                        employee_id = c.Employee_Record.employee_id,
                        joining_date = c.Employee_Record.joining_date,
                        offer_letter_path = c.Employee_Record.offer_letter_path,
                        job_id = c.Employee_Record.job_id
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

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            string resume_path = candidate.resume_path;

            return resume_path;
        }

        public async Task<CandidateDtos.CandidateDashboardProfile> GetCandidateDashProfile(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            var response = new CandidateDtos.CandidateDashboardProfile
            {
                candidate_id = candidate.candidate_id,
                full_name = candidate.full_name,
                email = candidate.email,
                phone = candidate.phone
            };

            return response;
        }

        public async Task<CandidateDtos.CandidateDashboardProfile> UpdateCandidateDashProfile(CandidateDtos.CandidateDashboardProfile dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate id");

            if(dto.email.IsNullOrEmpty() || dto.phone.IsNullOrEmpty() || dto.full_name.IsNullOrEmpty())
                throw new ArgumentException("Invalid Input for fields");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            candidate.full_name = dto.full_name;
            candidate.email = dto.email;
            candidate.phone = dto.phone;

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<Boolean> IsRegisteredAsync(string email)
        {
            if (email.IsNullOrEmpty())
                throw new ArgumentException("Invalid Email");

            var data = await _context.Candidates.FirstOrDefaultAsync(r => r.email == email);

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

            var data = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id);

            if (data != null)
            {
                throw new Exception("Candidate already exist");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            Candidate candidate = new Candidate
            {
                candidate_id = dto.candidate_id,
                full_name = dto.full_name,
                email = dto.email,
                phone = dto.phone,
                resume_path = dto.resume_path,
                created_at = DateTime.Now,
                password = hashedPassword,
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

            var data = await _context.Candidates.FirstOrDefaultAsync(r => r.email == dto.email);

            if (data != null)
            {
                throw new Exception("Email already in use");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            Candidate candidate = new Candidate
            {
                candidate_id = dto.candidate_id,
                full_name = dto.full_name,
                email = dto.email,
                phone = dto.phone,
                resume_path = "not provided",
                created_at = DateTime.Now,
                password = hashedPassword,
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
            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            candidate.password = hashedPassword;

            await _context.SaveChangesAsync();
            return true;
        }  

        public async Task<CandidateDtos.UploadCandidateResume> UploadCandidateResumeAsync(CandidateDtos.UploadCandidateResume dto)
        {
            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate Id");

            if (dto.resume_path.IsNullOrEmpty())
                throw new ArgumentException("Invalid Resume path");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            candidate.resume_path = dto.resume_path;

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
                    var existingCandidate = await _context.Candidates.FirstOrDefaultAsync(r => r.email == d.email);
                    if (existingCandidate != null)
                    {
                        throw new Exception($"Candidate with email {d.email} is already registered");
                    }

                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(d.password);

                    Candidate candidate = new Candidate
                    {
                        candidate_id = d.candidate_id,
                        full_name = d.full_name,
                        email = d.email,
                        phone = d.phone,
                        resume_path = d.resume_path,
                        created_at = DateTime.Now,
                        password = hashedPassword,
                    };

                    await _context.Candidates.AddAsync(candidate);
                    await _context.SaveChangesAsync();

                    insertedCandidateIds.Add(candidate.email);
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
                    var existingCandidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == d.candidate_id);

                    if (existingCandidate == null)
                    {
                        Console.WriteLine($"Candidate with ID {d.candidate_id} not found.");
                        continue;
                    }

                    // Update fields
                    existingCandidate.full_name = d.full_name;
                    existingCandidate.email = d.email;
                    existingCandidate.phone = d.phone;
                    existingCandidate.resume_path = d.resume_path;

                    _context.Candidates.Update(existingCandidate);
                    await _context.SaveChangesAsync();

                    updatedCandidateEmails.Add(existingCandidate.email);
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
                    var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.candidate_id == d.candidate_id);

                    if (candidate == null)
                    {
                        Console.WriteLine($"Candidate with ID {d.candidate_id} not found.");
                        continue;
                    }

                    _context.Candidates.Remove(candidate);
                    await _context.SaveChangesAsync();

                    deletedCandidateEmails.Add(candidate.candidate_id);
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

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == id);

            if (candidate == null)
                throw new KeyNotFoundException("Candidate not found with the specified ID.");

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
