using RecruitmentApi.Data;

namespace RecruitmentApi.Services
{
    public class CandidateDocService
    {
        AppDbContext _context;

        public CandidateDocService(AppDbContext context)
        {
            _context = context;
        }


    }
}
