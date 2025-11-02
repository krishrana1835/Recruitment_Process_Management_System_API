using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class Candidate_DocumentService
    {
        private AppDbContext _context;

        public Candidate_DocumentService(AppDbContext context)
        {
            _context = context;
        }

        public enum DocumentVerificationStatus
        {
            Pending,
            Verified,
            Rejected,
        }

        public async Task<List<Candidate_DocumentDtos.Candidate_DocumentDto>?> GetCandidateDocumentsAsync(string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Incalid Candidate id");

            var documents = await _context.Candidate_Documents.Where(r => r.candidate_id == candidate_id).ToListAsync();

            if (documents == null)
                return null;

            var response = documents.Select(r => new Candidate_DocumentDtos.Candidate_DocumentDto
            {
                document_id = r.document_id,
                document_type = r.document_type,
                verification_status = r.verification_status,
                file_path = r.file_path,
                uploaded_at = r.uploaded_at
            }).ToList();

            return response;
        }

        public async Task<Candidate_DocumentDtos.Candidate_DocumentDto> AddCandidateDocumentAsync(Candidate_DocumentDtos.UploadCandidateDocuments dto)
        {

            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Incalid candidate id");

            if (dto.document_type.IsNullOrEmpty() || dto.file_path.IsNullOrEmpty())
                throw new ArgumentException("One or more field is missing in request");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            var document = await _context.Candidate_Documents.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id && r.document_type == dto.document_type);
            
            if(document == null)
            {
                var data = new Candidate_Document
                {
                    document_type = dto.document_type,
                    file_path = dto.file_path,
                    candidate_id = dto.candidate_id,
                    verification_status = DocumentVerificationStatus.Pending.ToString(),
                    uploaded_at = DateTime.Now,
                };

                await _context.Candidate_Documents.AddAsync(data);
            }
            else
            {
                document.file_path = dto.file_path;
                document.verification_status = DocumentVerificationStatus.Pending.ToString();
                document.uploaded_at = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            
            var changes = await _context.Candidate_Documents.FirstOrDefaultAsync(r => r.candidate_id == dto.candidate_id && r.document_type == dto.document_type);

            if (changes == null!)
                throw new Exception("Changes not saved");

            var response = new Candidate_DocumentDtos.Candidate_DocumentDto
            {
                document_id = changes.document_id,
                document_type = changes.document_type,
                file_path = changes.file_path,
                verification_status = changes.verification_status,
                uploaded_at = changes.uploaded_at,
            };
            return response;
        } 
    }
}
