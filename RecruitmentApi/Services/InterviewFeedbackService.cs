using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Data;
using RecruitmentApi.Models;

namespace RecruitmentApi.Services
{
    public class InterviewFeedbackService
    {
        private AppDbContext _context;

        public InterviewFeedbackService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateCandidateFeedback(Interview_FeedBackDtos.InterviewSkillSubmissionDto req)
        {
            if (!await _context.Interviews.AnyAsync(i => i.interview_id == req.interview_id))
                throw new NullReferenceException("Interview does not exist");

            async Task ProcessSkills(List<Interview_FeedBackDtos.SkillReviewDto> skills)
            {
                foreach (var skill in skills)
                {
                    var candidateSkill = await _context.Candidate_Skills
                        .FirstOrDefaultAsync(cs =>
                            cs.candidate_id == req.candidate_id &&
                            cs.skill_id == skill.skill_id);

                    if (candidateSkill == null)
                    {
                        candidateSkill = new Candidate_Skill
                        {
                            years_experience = skill.review.yearsOfExperience,
                            skill_id = skill.skill_id,
                            candidate_id = req.candidate_id
                        };

                        _context.Candidate_Skills.Add(candidateSkill);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        candidateSkill.years_experience = skill.review.yearsOfExperience;
                        _context.Candidate_Skills.Update(candidateSkill);
                    }

                    var feedback = await _context.Interview_Feedbacks
                        .FirstOrDefaultAsync(f =>
                            f.interview_id == req.interview_id &&
                            f.user_id == req.user_id &&
                            f.candidate_skill_id == candidateSkill.candidate_skill_id);

                    if (feedback == null)
                    {
                        feedback = new Interview_Feedback
                        {
                            concept_rating = skill.review.conceptRating,
                            technical_rating = skill.review.technicalRating,
                            comments = skill.review.comments,
                            feedback_at = DateTime.Now,
                            interview_id = req.interview_id,
                            user_id = req.user_id,
                            candidate_skill_id = candidateSkill.candidate_skill_id
                        };

                        _context.Interview_Feedbacks.Add(feedback);
                    }
                    else
                    {
                        feedback.concept_rating = skill.review.conceptRating;
                        feedback.technical_rating = skill.review.technicalRating;
                        feedback.comments = skill.review.comments;
                        feedback.feedback_at = DateTime.Now;

                        _context.Interview_Feedbacks.Update(feedback);
                    }
                }

                await _context.SaveChangesAsync();
            }

            if (req.extra_skills.Any())
                await ProcessSkills(req.extra_skills);

            if (req.required_skills.Any())
                await ProcessSkills(req.required_skills);

            if (req.preferred_skills.Any())
                await ProcessSkills(req.preferred_skills);
        }

        public async Task<Interview_FeedBackDtos.InterviewSkillSubmissionDto> GetCandidateFeedbackForInterview(int interviewId, string userId)
        {
            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.interview_id == interviewId);

            if (interview == null)
                throw new NullReferenceException("Interview not found");

            string candidateId = interview.candidate_id;
            int jobId = interview.job_id;

            var feedbacks = await _context.Interview_Feedbacks
                .Where(f => f.interview_id == interviewId && f.user_id == userId)
                .ToListAsync();

            if (!feedbacks.Any())
                return new Interview_FeedBackDtos.InterviewSkillSubmissionDto
                {
                    interview_id = interviewId,
                    user_id = userId,
                    candidate_id = candidateId
                };

            var candidateSkillIds = feedbacks.Select(f => f.candidate_skill_id).ToList();

            var candidateSkills = await _context.Candidate_Skills
                .Where(cs => candidateSkillIds.Contains(cs.candidate_skill_id))
                .Include(cs => cs.skill)
                .ToListAsync();

            var jobSkills = await _context.Jobs_Skills
                .Where(js => js.job_id == jobId)
                .ToListAsync();

            var response = new Interview_FeedBackDtos.InterviewSkillSubmissionDto
            {
                interview_id = interviewId,
                user_id = userId,
                candidate_id = candidateId,
                extra_skills = new(),
                preferred_skills = new(),
                required_skills = new()
            };

            foreach (var feedback in feedbacks)
            {
                var candidateSkill = candidateSkills
                    .FirstOrDefault(cs => cs.candidate_skill_id == feedback.candidate_skill_id);

                if (candidateSkill == null)
                    continue;

                var jobSkill = jobSkills.FirstOrDefault(js => js.skill_id == candidateSkill.skill_id);
                string skillType = jobSkill?.skill_type ?? "E";

                var dto = new Interview_FeedBackDtos.SkillReviewDto
                {
                    skill_id = candidateSkill.skill_id,
                    skill_name = candidateSkill.skill.skill_name,
                    skill_type = skillType,
                    review = new Interview_FeedBackDtos.SkillReviewDataDto
                    {
                        yearsOfExperience = candidateSkill.years_experience,
                        conceptRating = feedback.concept_rating,
                        technicalRating = feedback.technical_rating,
                        comments = feedback.comments
                    }
                };

                switch (skillType)
                {
                    case "P":
                        response.required_skills.Add(dto);
                        break;

                    case "R":
                        response.preferred_skills.Add(dto);
                        break;

                    default:
                        response.extra_skills.Add(dto);
                        break;
                }
            }

            return response;
        }

    }
}
