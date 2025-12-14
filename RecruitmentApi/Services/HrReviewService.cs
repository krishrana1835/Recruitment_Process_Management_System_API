using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;

namespace RecruitmentApi.Services
{
    public class HrReviewService
    {
        private AppDbContext _context;

        public HrReviewService(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<HrReviewDtos.ReviewDto> AddHrReview(HrReviewDtos.ReviewDto req)
        //{
        //    if (req.strengths.IsNullOrEmpty() || req.areas_for_improvement.IsNullOrEmpty() ||
        //        req.training_recommendations.IsNullOrEmpty() || req.career_path_notes.IsNullOrEmpty())
        //        throw new ArgumentException("Invalid input");

        //    var review = await _context.HR_Re
        //}
    }
}
