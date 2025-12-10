using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentApi.Models
{
    public class HR_Review
    {
        [Key]
        public int review_id { get; set; }

        public int communication_rating { get; set; }

        public int teamwork_rating { get; set; }

        public int adaptability_rating { get; set; }

        public int leadership_rating { get; set; }

        public string strengths { get; set; } = null!;

        public string areas_for_improvement { get; set; } = null!;

        public string training_recommendations { get; set; } = null!;

        public string career_path_notes { get; set; } = null!;

        public int overall_rating { get; set; }

        public int interview_id { get; set; }

        public string user_id { get; set; } = null!;

        public Interview interview { get; set; } = null!;

        public virtual User user { get; set; } = null!;


    }
}
