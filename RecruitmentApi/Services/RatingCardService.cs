using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RecruitmentApi.Data;
using RecruitmentApi.Dtos;

namespace RecruitmentApi.Services
{
    public class RatingCardService
    {
        private AppDbContext _context;

        public RatingCardService(AppDbContext context) 
        {
            _context = context; 
        }

        public class RoundCardReq
        {
            public int job_id { get; set; }
            public int round_number { get; set; }
            public string candidate_id { get; set; } = null!;
        }

        public class RoundCardRes
        {
            public string user_id { get; set; } = null!;
            public string name { get; set; } = null!;
        }

        public class InterviewRoundRatingDto
        {
            public CandidateDtos.CandidateDto Candidate { get; set; } = new();
            public Interview_TypeDtos.InterviewType InterviewType { get; set; } = new();
            public List<UserDtos.UserDto> Users { get; set; } = new();

            public List<Interview_FeedBackDtos.InterveiwFeedbackCard> InterviewFeedbacks { get; set; } = new();

            public List<HrReviewDtos.Card> HrReviews { get; set; } = new();
        }
        public async Task<List<InterviewRoundRatingDto>> GetRatingByRoundAsync(RoundCardReq req)
        {
            if (!await _context.Jobs.AnyAsync(i => i.job_id == req.job_id))
                throw new Exception("Job does not exist");

            if (!await _context.Candidates.AnyAsync(i => i.candidate_id == req.candidate_id))
                throw new Exception("Candidate does not exist");

            var interviews = await _context.Interviews
                .AsNoTracking()
                .Where(i =>
                    i.job_id == req.job_id &&
                    i.candidate_id == req.candidate_id &&
                    i.round_number == req.round_number)
                .Select(i => new InterviewRoundRatingDto
                {
                    Candidate = new CandidateDtos.CandidateDto
                    {
                        candidate_id = i.candidate_id,
                        full_name = i.candidate.full_name,
                        email = i.candidate.email
                    },

                    InterviewType = new Interview_TypeDtos.InterviewType
                    {
                        interview_type_id = i.interview_type_id,
                        interview_round_name = i.interview_type.interview_round_name,
                        process_descreption = i.interview_type.process_descreption
                    },

                    Users = i.users.Select(u => new UserDtos.UserDto
                    {
                        user_id = u.user_id,
                        name = u.name,
                        email = u.email
                    }).ToList(),

                    InterviewFeedbacks = i.Interview_Feedbacks.Select(f => new Interview_FeedBackDtos.InterveiwFeedbackCard
                    {
                        concept_rating = f.concept_rating,
                        technical_rating = f.technical_rating,
                        candidate_skill_id = f.candidate_skill_id,
                        user_id = f.user_id,
                        Candidaete_Skill = new Candidate_SkillDtos.Candidate_SkillDto
                        {
                            candidate_id = f.candidate_skill.candidate_id,
                            years_experience = f.candidate_skill.years_experience,
                            skill_id = f.candidate_skill.skill_id,
                            candidate_skill_id = f.candidate_skill_id,
                            skill = new SkillDtos.SkillDto
                            {
                                skill_id = f.candidate_skill.skill.skill_id,
                                skill_name = f.candidate_skill.skill.skill_name
                            }
                        }
                    }).ToList(),

                    HrReviews = i.HR_Reviews.Select(h => new HrReviewDtos.Card
                    {
                        user_id = h.user_id,
                        adaptability_rating = h.adaptability_rating,
                        teamwork_rating = h.teamwork_rating,
                        communication_rating = h.communication_rating,
                        leadership_rating = h.leadership_rating,
                        overall_rating = h.overall_rating
                    }).ToList()
                })
                .ToListAsync();

            return interviews;
        }

    }
}
