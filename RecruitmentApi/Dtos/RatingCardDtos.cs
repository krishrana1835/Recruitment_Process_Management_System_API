namespace RecruitmentApi.Dtos
{
    public class RatingCardDtos
    {
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
            public float score { get; set; }
            public CandidateDtos.CandidateDto Candidate { get; set; } = new();
            public Interview_TypeDtos.InterviewType InterviewType { get; set; } = new();
            public List<UserDtos.UserDto> Users { get; set; } = new();

            public List<Interview_FeedBackDtos.InterveiwFeedbackCard> InterviewFeedbacks { get; set; } = new();

            public List<HrReviewDtos.Card> HrReviews { get; set; } = new();
        }

        public class ListCanndidateScores
        {
            public int interview_id { get; set; }
            public float score { get; set; }
            public string status { get; set; } = null!;
            public CandidateDtos.CandidateDto Candidate { get; set; } = new();
        }
    }
}
