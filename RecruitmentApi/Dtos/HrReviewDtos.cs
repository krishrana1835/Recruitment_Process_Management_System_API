namespace RecruitmentApi.Dtos
{
    public class HrReviewDtos
    {
        public class ReviewDto
        {
            public int adaptability_rating { get; set; }
            public int communication_rating { get; set; }
            public int leadership_rating { get; set; }
            public int overall_rating { get; set; }
            public int teamwork_rating {get; set;}
            public string strengths { get; set; } = null!;
            public string career_path_notes { get; set; } = null!;
            public string areas_for_improvement { get; set; } = null!;
            public string training_recommendations { get; set; } = null!;
            public int interview_id { get; set; }
            public string user_id { get; set; } = null!;
        }

        public class Card
        {
            public string user_id { get; set; } = null!;
            public int adaptability_rating { get; set; }
            public int communication_rating { get; set; }
            public int leadership_rating { get; set; }
            public int overall_rating { get; set; }
            public int teamwork_rating { get; set; }
        }
    }
}
