namespace RecruitmentApi.Dtos
{
    public class Employee_RecordDtos
    {
        public class Employee_RecordDto_Candidate
        {
            public int employee_id { get; set; }

            public DateOnly joining_date { get; set; }

            public string offer_letter_path { get; set; } = null!;

            public int job_id { get; set; }
        }

        public class EmployeeInsertDto
        {
            public DateOnly JoiningDate { get; set; }

            public string OfferLetterPath { get; set; } = null!;

            public string CandidateId { get; set; } = null!;

            public int JobId { get; set; }
            public string UserId { get; set; } = null!;
        }

        public class FetchEmployeeDto
        {
            public int EmployeeId { get; set; }
            public DateOnly JoiningDate { get; set; }
            public string OfferLetterPath { get; set; } = null!;
            public CandidateDtos.CandidateDto Candidate { get; set; } = new();
        }
        public class EmployeeUpdateDto
        {
            public int EmployeeId { get; set; }
            public DateOnly JoiningDate { get; set; }
            public string OfferLetterPath { get; set; } = null!;
        }
        public class OfferLatterCandidate
        {
            public int EmployeeId { get; set; }

            public DateOnly JoiningDate { get; set; }

            public string OfferLetterPath { get; set; } = null!;

            public DisplayJob Job { get; set; } = null!;
        }

        public class DisplayJob
        {
            public int JobId { get; set; }
            public string JobTitle { get; set; } = null!;
            public string JobDescription { get; set; } = null!;
        }
    }
}
