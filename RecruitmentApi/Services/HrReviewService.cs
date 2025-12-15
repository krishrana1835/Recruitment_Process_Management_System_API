using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class HrReviewService
    {
        private AppDbContext _context;

        public HrReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HrReviewDtos.ReviewDto?> getHrReview(int interview_id, string user_id)
        {
            if (!(await _context.Interviews.AnyAsync(i => i.interview_id == interview_id))) throw new NullReferenceException("Interview does not exist");

            if (!(await _context.Users.AnyAsync(u => u.user_id == user_id))) throw new NullReferenceException("User does not exist");

            var review = await _context.HR_Reviews.FirstOrDefaultAsync(i => i.interview_id == interview_id && i.user_id == user_id);

            if (review == null)
                return null;

            return new HrReviewDtos.ReviewDto
            {
                communication_rating = review.communication_rating,
                teamwork_rating = review.teamwork_rating,
                adaptability_rating = review.adaptability_rating,
                leadership_rating = review.leadership_rating,
                strengths = review.strengths,
                areas_for_improvement = review.areas_for_improvement,
                training_recommendations = review.training_recommendations,
                career_path_notes = review.career_path_notes,
                overall_rating = review.overall_rating,
                interview_id = review.interview_id,
                user_id = review.user_id,
            };
        }

        public async Task<HrReviewDtos.ReviewDto> AddOrUpdateHrReview(HrReviewDtos.ReviewDto req)
        {
            if (req.strengths.IsNullOrEmpty() || req.areas_for_improvement.IsNullOrEmpty() ||
                req.training_recommendations.IsNullOrEmpty() || req.career_path_notes.IsNullOrEmpty())
                throw new ArgumentException("Invalid input");

            var review = await _context.HR_Reviews.FirstOrDefaultAsync(i => i.user_id == req.user_id && i.interview_id == req.interview_id);

            if(review == null)
            {
                var newReview = new HR_Review
                {
                    communication_rating = req.communication_rating,
                    teamwork_rating = req.teamwork_rating,
                    adaptability_rating = req.adaptability_rating,
                    leadership_rating = req.leadership_rating,
                    strengths = req.strengths,
                    areas_for_improvement = req.areas_for_improvement,
                    training_recommendations = req.training_recommendations,
                    career_path_notes = req.career_path_notes,
                    overall_rating = req.overall_rating,
                    interview_id = req.interview_id,
                    user_id = req.user_id,
                };

                await _context.HR_Reviews.AddAsync(newReview);
            }else
            {
                review.communication_rating = req.communication_rating;
                review.teamwork_rating = req.teamwork_rating;
                review.adaptability_rating = req.adaptability_rating;
                review.leadership_rating = req.leadership_rating;
                review.strengths = req.strengths;
                review.areas_for_improvement = req.areas_for_improvement;
                review.training_recommendations = req.training_recommendations;
                review.career_path_notes = req.career_path_notes;
                review.overall_rating = req.overall_rating;
            }

            await _context.SaveChangesAsync();

            return req;
        }
    }
}
