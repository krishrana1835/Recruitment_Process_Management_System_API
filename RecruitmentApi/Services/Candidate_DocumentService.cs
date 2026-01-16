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

            var documents = await _context.Candidate_Documents.Where(r => r.CandidateId == candidate_id).ToListAsync();

            if (documents == null)
                return null;

            var response = documents.Select(r => new Candidate_DocumentDtos.Candidate_DocumentDto
            {
                document_id = r.DocumentId,
                document_type = r.DocumentType,
                verification_status = r.VerificationStatus,
                file_path = r.FilePath,
                uploaded_at = r.UploadedAt
            }).ToList();

            return response;
        }

        public async Task<Candidate_DocumentDtos.Candidate_DocumentDto> AddCandidateDocumentAsync(Candidate_DocumentDtos.UploadCandidateDocuments dto)
        {

            if (dto.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Incalid candidate id");

            if (dto.document_type.IsNullOrEmpty() || dto.file_path.IsNullOrEmpty())
                throw new ArgumentException("One or more field is missing in request");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id);

            if (candidate == null)
                throw new Exception("Candidate not found");

            var document = await _context.Candidate_Documents.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id && r.DocumentType == dto.document_type);
            
            if(document == null)
            {
                var data = new Candidate_Document
                {
                    DocumentType = dto.document_type,
                    FilePath = dto.file_path,
                    CandidateId = dto.candidate_id,
                    VerificationStatus = DocumentVerificationStatus.Pending.ToString(),
                    UploadedAt = DateTime.Now,
                };

                await _context.Candidate_Documents.AddAsync(data);
            }
            else
            {
                document.FilePath = dto.file_path;
                document.VerificationStatus = DocumentVerificationStatus.Pending.ToString();
                document.UploadedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            
            var changes = await _context.Candidate_Documents.FirstOrDefaultAsync(r => r.CandidateId == dto.candidate_id && r.DocumentType == dto.document_type);

            if (changes == null!)
                throw new Exception("Changes not saved");

            var response = new Candidate_DocumentDtos.Candidate_DocumentDto
            {
                document_id = changes.DocumentId,
                document_type = changes.DocumentType,
                file_path = changes.FilePath,
                verification_status = changes.VerificationStatus,
                uploaded_at = changes.UploadedAt,
            };
            return response;
        } 

        public async Task<Boolean> FetchDocUploadStatus(string candidate_id)
        {
            if (candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(i => i.CandidateId == candidate_id);

            if (candidate == null)
                throw new NullReferenceException("Caniddate does not exist");

            return candidate.DocUpload;
        }

        public async Task<Candidate_DocumentDtos.ChangeStatusReq> UpdateDocUploadStatus(Candidate_DocumentDtos.ChangeStatusReq req)
        {
            if (req.candidate_id.IsNullOrEmpty())
                throw new ArgumentException("Invalid candidate id");

            var candidate = await _context.Candidates.FirstOrDefaultAsync(i => i.CandidateId == req.candidate_id);

            if (candidate == null)
                throw new NullReferenceException("Caniddate does not exist");

            candidate.DocUpload = req.doc_upload;

            await _context.SaveChangesAsync();

            return req;
        }

        public async Task<Candidate_DocumentDtos.VerificationStatus> UpdateVerificationStatus(Candidate_DocumentDtos.VerificationStatus req)
        {
            if (!(req.verification_status == "Pending" || req.verification_status == "Verified" || req.verification_status == "Rejected"))
                throw new InvalidOperationException("Invalid status");

            var doc = await _context.Candidate_Documents.FirstOrDefaultAsync(i => i.DocumentId == req.document_id);

            if (doc == null)
                throw new NullReferenceException("Document does not exist");

            doc.VerificationStatus = req.verification_status;

            if(req.verification_status == "Verified" || req.verification_status == "Rejected")
            {
                var status_history = new Candidate_Status_History
                {
                    Status = req.verification_status,
                    Reason = $"{doc.DocumentType} is {req.verification_status} by HR.",
                    ChangedAt = DateTime.Now,
                    CandidateId = doc.CandidateId,
                    JobId = req.job_id,
                    ChangedBy = req.user_id,
                };

                await _context.Candidate_Status_Histories.AddAsync(status_history);
            }

            await _context.SaveChangesAsync();

            return req;
        }
    }
}
