using System.CodeDom;
using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class EmployeeRecordService
    {
        private AppDbContext _context;

        public EmployeeRecordService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddEmployeeAsync(Employee_RecordDtos.EmployeeInsertDto req)
        {
            if (await _context.Employee_Records.AnyAsync(emp => emp.CandidateId == req.CandidateId))
                throw new InvalidOperationException("Canidate is already an employee");

            var employee = new Employee_Record
            {
                JoiningDate = req.JoiningDate,
                OfferLetterPath= req.OfferLetterPath,
                JobId = req.JobId,
                CandidateId = req.CandidateId,
            };

            _context.Employee_Records.Add(employee);

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == req.JobId) ?? throw new NullReferenceException("Job does not exist");

            var status_history = new Candidate_Status_History
            {
                Status = "Hired",
                Reason = $"You are hired as an Employee for {job.JobTitle}",
                ChangedAt = DateTime.Now,
                CandidateId = req.CandidateId,
                JobId = req.JobId,
                ChangedBy = req.UserId,
            };

            await _context.Candidate_Status_Histories.AddAsync(status_history);

            await _context.SaveChangesAsync();
        }

        public async Task<List<CandidateDtos.SelectedCandiadte>> FetchCandidatesForEmployee(int job_id)
        {
            if (!await _context.Jobs.AnyAsync(i => i.JobId == job_id))
                throw new NullReferenceException("Job does not exist");

            var lastRoundNumber = await _context.Interviews
                .Where(i => i.JobId == job_id)
                .MaxAsync(i => (int?)i.RoundNumber);

            var candidates = await _context.Interviews
                .Where(i =>
                    i.JobId == job_id &&
                    i.Status == "Selected" &&
                    i.RoundNumber == lastRoundNumber &&
                    !_context.Employee_Records.Any(e => e.CandidateId == i.CandidateId)
                )
                .Select(r => new CandidateDtos.SelectedCandiadte
                {
                    candidate_id = r.CandidateId,
                    full_name = r.Candidate.FullName,
                    email = r.Candidate.Email,
                    doc_upload = r.Candidate.DocUpload
                })
                .ToListAsync();

            return candidates;
        }

        public async Task<List<Employee_RecordDtos.FetchEmployeeDto>> FetchEmployeesAsync(int job_id)
        {
            if (!await _context.Jobs.AnyAsync(j => j.JobId == job_id))
                throw new NullReferenceException("Job does not exist");

            return await _context.Employee_Records.Where(j => j.JobId == job_id)
                .Select(s => new Employee_RecordDtos.FetchEmployeeDto
                {
                    EmployeeId = s.EmployeeId,
                    JoiningDate = s.JoiningDate,
                    OfferLetterPath = s.OfferLetterPath,
                    Candidate = new CandidateDtos.CandidateDto
                    {
                        candidate_id = s.Candidate.CandidateId,
                        full_name = s.Candidate.FullName,
                        email = s.Candidate.Email,
                    }
                }).ToListAsync();
        }

        public async Task<Boolean> IsEmployeeAsync (string CandidateId)
        {
            return await _context.Employee_Records.AnyAsync(c => c.CandidateId == CandidateId);
        }

        public async Task<Employee_RecordDtos.OfferLatterCandidate> FetchOfferLatterAsync(string CandidateId)
        {
            return await _context.Employee_Records.Where(c => c.CandidateId == CandidateId)
                .Select(s => new Employee_RecordDtos.OfferLatterCandidate
                {
                    EmployeeId = s.EmployeeId,
                    JoiningDate= s.JoiningDate,
                    OfferLetterPath = s.OfferLetterPath,
                    Job = new Employee_RecordDtos.DisplayJob
                    {
                        JobId = s.JobId,
                        JobTitle = s.Job.JobTitle,
                        JobDescription = s.Job.JobDescription,
                    }
                }).FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Candidate is not an employee");
        }

        public async Task UpdateEmployeeAsync(Employee_RecordDtos.EmployeeUpdateDto req)
        {
            var employee = await _context.Employee_Records.FirstOrDefaultAsync(i => i.EmployeeId == req.EmployeeId) 
                ?? throw new KeyNotFoundException("Employee does not exist");

            employee.JoiningDate = req.JoiningDate;
            employee.OfferLetterPath = req.OfferLetterPath;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int EmployeeId)
        {
            var employee = await _context.Employee_Records.FirstOrDefaultAsync(e => e.EmployeeId == EmployeeId) 
                ?? throw new KeyNotFoundException("Employee does not exist");

            _context.Employee_Records.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}
