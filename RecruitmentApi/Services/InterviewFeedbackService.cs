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
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.InterviewId == req.interview_id);
            if (interview == null)
                throw new NullReferenceException("Interview does not exist");
            interview.Score = req.total_score;
            await _context.SaveChangesAsync();

            async Task ProcessSkills(List<Interview_FeedBackDtos.SkillReviewDto> skills)
            {
                foreach (var skill in skills)
                {
                    var candidateSkill = await _context.Candidate_Skills
                        .FirstOrDefaultAsync(cs =>
                            cs.CandidateId == req.candidate_id &&
                            cs.SkillId == skill.skill_id);

                    if (candidateSkill == null)
                    {
                        candidateSkill = new Candidate_Skill
                        {
                            YearsExperience = skill.review.yearsOfExperience,
                            SkillId = skill.skill_id,
                            CandidateId = req.candidate_id
                        };

                        _context.Candidate_Skills.Add(candidateSkill);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        candidateSkill.YearsExperience = skill.review.yearsOfExperience;
                        _context.Candidate_Skills.Update(candidateSkill);
                    }

                    var feedback = await _context.Interview_Feedbacks
                        .FirstOrDefaultAsync(f =>
                            f.InterviewId == req.interview_id &&
                            f.UserId == req.user_id &&
                            f.CandidateSkillId == candidateSkill.CandidateSkillId);

                    if (feedback == null)
                    {
                        feedback = new Interview_Feedback
                        {
                            ConceptRating = skill.review.conceptRating,
                            TechnicalRating = skill.review.technicalRating,
                            Comments = skill.review.comments,
                            FeedbackAt = DateTime.Now,
                            InterviewId = req.interview_id,
                            UserId = req.user_id,
                            CandidateSkillId = candidateSkill.CandidateSkillId
                        };

                        _context.Interview_Feedbacks.Add(feedback);
                    }
                    else
                    {
                        feedback.ConceptRating = skill.review.conceptRating;
                        feedback.TechnicalRating = skill.review.technicalRating;
                        feedback.Comments = skill.review.comments;
                        feedback.FeedbackAt = DateTime.Now;

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
                .FirstOrDefaultAsync(i => i.InterviewId == interviewId);

            if (interview == null)
                throw new NullReferenceException("Interview not found");

            string candidateId = interview.CandidateId;
            int jobId = interview.JobId;

            var feedbacks = await _context.Interview_Feedbacks
                .Where(f => f.InterviewId == interviewId && f.UserId == userId)
                .ToListAsync();

            if (!feedbacks.Any())
                return new Interview_FeedBackDtos.InterviewSkillSubmissionDto
                {
                    interview_id = interviewId,
                    user_id = userId,
                    candidate_id = candidateId
                };

            var candidateSkillIds = feedbacks.Select(f => f.CandidateSkillId).ToList();

            var candidateSkills = await _context.Candidate_Skills
                .Where(cs => candidateSkillIds.Contains(cs.CandidateSkillId))
                .Include(cs => cs.Skill)
                .ToListAsync();

            var jobSkills = await _context.Jobs_Skills
                .Where(js => js.JobId == jobId)
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
                    .FirstOrDefault(cs => cs.CandidateSkillId == feedback.CandidateSkillId);

                if (candidateSkill == null)
                    continue;

                var jobSkill = jobSkills.FirstOrDefault(js => js.SkillId == candidateSkill.SkillId);
                string skillType = jobSkill?.SkillType ?? "E";

                var dto = new Interview_FeedBackDtos.SkillReviewDto
                {
                    skill_id = candidateSkill.SkillId,
                    skill_name = candidateSkill.Skill.SkillName,
                    skill_type = skillType,
                    review = new Interview_FeedBackDtos.SkillReviewDataDto
                    {
                        yearsOfExperience = candidateSkill.YearsExperience,
                        conceptRating = feedback.ConceptRating,
                        technicalRating = feedback.TechnicalRating,
                        comments = feedback.Comments
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
