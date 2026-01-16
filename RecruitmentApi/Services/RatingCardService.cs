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

        public async Task<List<RatingCardDtos.InterviewRoundRatingDto>> GetRatingByRoundAsync(RatingCardDtos.RoundCardReq req)
        {
            if (!await _context.Jobs.AnyAsync(i => i.JobId == req.job_id))
                throw new Exception("Job does not exist");

            if (!await _context.Candidates.AnyAsync(i => i.CandidateId == req.candidate_id))
                throw new Exception("Candidate does not exist");

            var interviews = await _context.Interviews
                .AsNoTracking()
                .Where(i =>
                    i.JobId == req.job_id &&
                    i.CandidateId == req.candidate_id &&
                    i.RoundNumber == req.round_number)
                .Select(i => new RatingCardDtos.InterviewRoundRatingDto
                {
                    Candidate = new CandidateDtos.CandidateDto
                    {
                        candidate_id = i.CandidateId,
                        full_name = i.Candidate.FullName,
                        email = i.Candidate.Email
                    },

                    InterviewType = new Interview_TypeDtos.InterviewType
                    {
                        interview_type_id = i.InterviewTypeId,
                        interview_round_name = i.InterviewType.InterviewRoundName,
                        process_descreption = i.InterviewType.ProcessDescreption
                    },

                    Users = i.Users.Select(u => new UserDtos.UserDto
                    {
                        user_id = u.UserId,
                        name = u.Name,
                        email = u.Email
                    }).ToList(),

                    InterviewFeedbacks = i.InterviewFeedbacks.Select(f => new Interview_FeedBackDtos.InterveiwFeedbackCard
                    {
                        concept_rating = f.ConceptRating,
                        technical_rating = f.TechnicalRating,
                        candidate_skill_id = f.CandidateSkillId,
                        user_id = f.UserId,
                        Candidaete_Skill = new Candidate_SkillDtos.Candidate_SkillDto
                        {
                            candidate_id = f.CandidateSkill.CandidateId,
                            years_experience = f.CandidateSkill.YearsExperience,
                            skill_id = f.CandidateSkill.SkillId,
                            candidate_skill_id = f.CandidateSkillId,
                            skill = new SkillDtos.SkillDto
                            {
                                skill_id = f.CandidateSkill.Skill.SkillId,
                                skill_name = f.CandidateSkill.Skill.SkillName
                            }
                        }
                    }).ToList(),

                    HrReviews = i.HrReviews.Select(h => new HrReviewDtos.Card
                    {
                        user_id = h.UserId,
                        adaptability_rating = h.AdaptabilityRating,
                        teamwork_rating = h.TeamworkRating,
                        communication_rating = h.CommunicationRating,
                        leadership_rating = h.LeadershipRating,
                        overall_rating = h.OverallRating
                    }).ToList()
                })
                .ToListAsync();

            return interviews;
        }

        public async Task<List<RatingCardDtos.ListCanndidateScores>> GetCandidateWithScore(int job_id, int round_number)
        {
            if (!await _context.Jobs.AnyAsync())
                throw new NullReferenceException("Job does not exist");

            var interview = await _context.Interviews.Where(i => i.JobId == job_id && i.RoundNumber == round_number)
                .Select(s => new RatingCardDtos.ListCanndidateScores
                {
                    interview_id = s.InterviewId,
                    score = s.Score,
                    status = s.Status,
                    Candidate = new CandidateDtos.CandidateDto
                    {
                        candidate_id = s.CandidateId,
                        full_name = s.Candidate.FullName,
                        email = s.Candidate.Email,
                    }
                }).ToListAsync();

            return interview;
        }

        public async Task<List<RatingCardDtos.InterviewRoundRatingDto>> GetRatingsAsync(int job_id, int round_number, string candidate_id)
        {
            if (!await _context.Jobs.AnyAsync(i => i.JobId == job_id))
                throw new Exception("Job does not exist");

            if (!await _context.Candidates.AnyAsync(i => i.CandidateId == candidate_id))
                throw new Exception("Candidate does not exist");

            var interviews = await _context.Interviews
                .AsNoTracking()
                .Where(i =>
                    i.JobId == job_id &&
                    i.CandidateId == candidate_id &&
                    i.RoundNumber <= round_number)
                .Select(i => new RatingCardDtos.InterviewRoundRatingDto
                {
                    Candidate = new CandidateDtos.CandidateDto
                    {
                        candidate_id = i.CandidateId,
                        full_name = i.Candidate.FullName,
                        email = i.Candidate.Email
                    },

                    InterviewType = new Interview_TypeDtos.InterviewType
                    {
                        interview_type_id = i.InterviewTypeId,
                        interview_round_name = i.InterviewType.InterviewRoundName,
                        process_descreption = i.InterviewType.ProcessDescreption
                    },

                    Users = i.Users.Select(u => new UserDtos.UserDto
                    {
                        user_id = u.UserId,
                        name = u.Name,
                        email = u.Email
                    }).ToList(),

                    InterviewFeedbacks = i.InterviewFeedbacks.Select(f => new Interview_FeedBackDtos.InterveiwFeedbackCard
                    {
                        concept_rating = f.ConceptRating,
                        technical_rating = f.TechnicalRating,
                        candidate_skill_id = f.CandidateSkillId,
                        user_id = f.UserId,
                        Candidaete_Skill = new Candidate_SkillDtos.Candidate_SkillDto
                        {
                            candidate_id = f.CandidateSkill.CandidateId,
                            years_experience = f.CandidateSkill.YearsExperience,
                            skill_id = f.CandidateSkill.SkillId,
                            candidate_skill_id = f.CandidateSkillId,
                            skill = new SkillDtos.SkillDto
                            {
                                skill_id = f.CandidateSkill.Skill.SkillId,
                                skill_name = f.CandidateSkill.Skill.SkillName
                            }
                        }
                    }).ToList(),

                    HrReviews = i.HrReviews.Select(h => new HrReviewDtos.Card
                    {
                        user_id = h.UserId,
                        adaptability_rating = h.AdaptabilityRating,
                        teamwork_rating = h.TeamworkRating,
                        communication_rating = h.CommunicationRating,
                        leadership_rating = h.LeadershipRating,
                        overall_rating = h.OverallRating
                    }).ToList()
                })
                .ToListAsync();

            return interviews;
        }

    }
}
