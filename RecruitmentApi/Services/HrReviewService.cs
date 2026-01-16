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
            if (!(await _context.Interviews.AnyAsync(i => i.InterviewId == interview_id))) throw new NullReferenceException("Interview does not exist");

            if (!(await _context.Users.AnyAsync(u => u.UserId == user_id))) throw new NullReferenceException("User does not exist");

            var review = await _context.HR_Reviews.FirstOrDefaultAsync(i => i.InterviewId == interview_id && i.UserId == user_id);

            if (review == null)
                return null;

            return new HrReviewDtos.ReviewDto
            {
                communication_rating = review.CommunicationRating,
                teamwork_rating = review.TeamworkRating,
                adaptability_rating = review.AdaptabilityRating,
                leadership_rating = review.LeadershipRating,
                strengths = review.Strengths,
                areas_for_improvement = review.AreasForImprovement,
                training_recommendations = review.TrainingRecommendations,
                career_path_notes = review.CareerPathNotes,
                overall_rating = review.OverallRating,
                interview_id = review.InterviewId,
                user_id = review.UserId,
            };
        }

        public async Task<HrReviewDtos.ReviewDto> AddOrUpdateHrReview(HrReviewDtos.ReviewDto req)
        {
            if (req.strengths.IsNullOrEmpty() || req.areas_for_improvement.IsNullOrEmpty() ||
                req.training_recommendations.IsNullOrEmpty() || req.career_path_notes.IsNullOrEmpty())
                throw new ArgumentException("Invalid input");

            var interveiw = await _context.Interviews.FirstOrDefaultAsync(i => i.InterviewId == req.interview_id);
            if (interveiw == null)
                throw new NullReferenceException("Interview does not exist");

            interveiw.Score = req.total_score;

            var review = await _context.HR_Reviews.FirstOrDefaultAsync(i => i.UserId == req.user_id && i.InterviewId == req.interview_id);

            if(review == null)
            {
                var newReview = new HR_Review
                {
                    CommunicationRating = req.communication_rating,
                    TeamworkRating = req.teamwork_rating,
                    AdaptabilityRating = req.adaptability_rating,
                    LeadershipRating = req.leadership_rating,
                    Strengths = req.strengths,
                    AreasForImprovement = req.areas_for_improvement,
                    TrainingRecommendations = req.training_recommendations,
                    CareerPathNotes = req.career_path_notes,
                    OverallRating = req.overall_rating,
                    InterviewId = req.interview_id,
                    UserId = req.user_id,
                };

                await _context.HR_Reviews.AddAsync(newReview);
            }else
            {
                review.CommunicationRating = req.communication_rating;
                review.TeamworkRating = req.teamwork_rating;
                review.AdaptabilityRating = req.adaptability_rating;
                review.LeadershipRating = req.leadership_rating;
                review.Strengths = req.strengths;
                review.AreasForImprovement = req.areas_for_improvement;
                review.TrainingRecommendations = req.training_recommendations;
                review.CareerPathNotes = req.career_path_notes;
                review.OverallRating = req.overall_rating;
            }

            await _context.SaveChangesAsync();

            return req;
        }
    }
}
