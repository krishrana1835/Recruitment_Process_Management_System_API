using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models
{
    public class HR_Review
    {
        public int ReviewId { get; set; }

        public int CommunicationRating { get; set; }

        public int TeamworkRating { get; set; }

        public int AdaptabilityRating { get; set; }

        public int LeadershipRating { get; set; }

        public string Strengths { get; set; } = null!;

        public string AreasForImprovement { get; set; } = null!;

        public string TrainingRecommendations { get; set; } = null!;

        public string CareerPathNotes { get; set; } = null!;

        public int OverallRating { get; set; }

        public int InterviewId { get; set; }

        public string UserId { get; set; } = null!;

        public Interview Interview { get; set; } = null!;

        public virtual User User { get; set; } = null!;


    }
}
