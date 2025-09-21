namespace RecruitmentApi.Dtos;

/// <summary>
/// Represents a data transfer object for role-related information.
/// </summary>
public class RoleDtos
{
    /// <summary>
    /// Represents a single role.
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the role.
        /// </summary>
        public int role_id { get; set; }
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        public string role_name { get; set; } = null!;
    }
}
