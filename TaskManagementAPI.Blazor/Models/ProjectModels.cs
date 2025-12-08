namespace TaskManagementAPI.Blazor.Models
{
    /// <summary>
    /// DTO for project response from API
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public string OwnerUsername { get; set; } = string.Empty;
        public int TaskCount { get; set; }
    }

    /// <summary>
    /// DTO for creating a new project
    /// </summary>
    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing project
    /// </summary>
    public class UpdateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
