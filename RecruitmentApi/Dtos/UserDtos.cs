using RecruitmentApi.Models;
using static RecruitmentApi.Dtos.RoleDtos;

namespace RecruitmentApi.Dtos;

/// <summary>
/// Represents a collection of data transfer objects for user-related information.
/// </summary>
public class UserDtos
{
    /// <summary>
    /// Represents a basic user data transfer object.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string name { get; set; } = null!;
        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string email { get; set; } = null!;
    }

    /// <summary>
    /// Represents a data transfer object for creating a new user.
    /// </summary>
    public class UserCreateDto : UserDto
    {
        /// <summary>
        /// Gets or sets the password for the new user.
        /// </summary>
        public string password { get; set; } = null!;
        /// <summary>
        /// Gets or sets the list of roles assigned to the new user.
        /// </summary>
        public List<RoleDto> roles { get; set; } = new();
    }

    /// <summary>
    /// Represents a data transfer object for updating an existing user.
    /// </summary>
    public class UserUpdateDto : UserDto
    {
        /// <summary>
        /// Gets or sets the list of roles assigned to the user.
        /// </summary>
        public List<RoleDto> roles { get; set; } = new();
    }

    /// <summary>
    /// Represents a data transfer object for basic user information.
    /// </summary>
    public class UserInfoDto : UserDto
    {
        /// <summary>
        /// Gets or sets the creation timestamp of the user.
        /// </summary>
        public DateTime created_at { get; set; }
        /// <summary>
        /// Gets or sets the list of roles associated with the user.
        /// </summary>
        public List<RoleDtos.RoleDto> roles { get; set; } = new();
    }

    /// <summary>
    /// Represents a data transfer object for a detailed user profile.
    /// </summary>
    public class UserProfileDto : UserDto
    {
        /// <summary>
        /// Gets or sets the creation timestamp of the user.
        /// </summary>
        public DateTime created_at { get; set; }

        /// <summary>
        /// Gets or sets the list of roles associated with the user.
        /// </summary>
        public List<RoleDtos.RoleDto> roles { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of interview feedbacks for the user.
        /// </summary>
        public List<Interview_FeedBackDtos.Interview_FeedbackDto?> interview_feedbacks { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of candidate reviews for the user.
        /// </summary>
        public List<Candidate_ReviewDtos.Candidate_ReviewDto?> candidate_reviews { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of jobs created by the user.
        /// </summary>
        public List<JobDtos.JobDto?> jobs_created { get; set; } = new();
    }
}