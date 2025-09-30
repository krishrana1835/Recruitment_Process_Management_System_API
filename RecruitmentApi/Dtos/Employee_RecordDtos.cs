namespace RecruitmentApi.Dtos
{
    public class Employee_RecordDtos
    {
        public class Employee_RecordDto_Candidate
        {
            public string employee_id { get; set; } = null!;

            public DateOnly joining_date { get; set; }

            public string offer_letter_path { get; set; } = null!;

            public int job_id { get; set; }
        }
    }
}
