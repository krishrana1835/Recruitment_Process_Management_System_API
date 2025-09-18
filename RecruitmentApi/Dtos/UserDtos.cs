using RecruitmentApi.Models;

namespace RecruitmentApi.Dtos;

public class UserDtos
{
    public class UserDto
    {
        public string user_id { get; set; }
        public string name { get; set; } = null!;
        public string email { get; set; } = null!;
    }

    public class UserCreateDto : UserDto
    {
        public string password { get; set; } = null!;
    }

    public class UserInfoDto : UserDto
    {
        public DateTime created_at { get; set; }
        public List<RoleDtos.RoleDto> roles { get; set; } = new();
    }

    public class UserProfileDto : UserDto
    {
        public DateTime created_at { get; set; }

        public List<RoleDtos.RoleDto> roles { get; set; } = new();

        public List<Interview_FeedBackDtos.Interview_FeedbackDto> interview_feedbacks { get; set; } = new();

        public List<Candidate_ReviewDtos.Candidate_ReviewDto> candidate_reviews { get; set; } = new();

        public List<JobDtos.JobDto> jobs_created { get; set; } = new();
    }
}