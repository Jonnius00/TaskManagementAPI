using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateProjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public string OwnerUsername { get; set; } = default!;
        public int TaskCount { get; set; }
    }
}

