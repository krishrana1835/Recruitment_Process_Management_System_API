using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos
{
    public class Candidate_DocumentDtos
    {
        public class Candidate_DocumentDto
        {
            public int document_id { get; set; }

            public string document_type { get; set; } = null!;

            public string file_path { get; set; } = null!;

            public string verification_status { get; set; } = null!;

            public DateTime uploaded_at { get; set; }
        }

        public class UploadCandidateDocuments
        {

            public string document_type { get; set; } = null!;

            public string file_path { get; set; } = null!;

            public string candidate_id { get; set; } = null!;

        }
    }
}
