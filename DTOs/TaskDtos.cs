using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public Models.TaskStatus Status { get; set; } = Models.TaskStatus.ToDo;

        public DateTime DueDate { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = default!;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public Models.TaskStatus Status { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int? AssignedUserId { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        [Required]
        public Models.TaskStatus Status { get; set; }
    }

    public class AssignTaskDto
    {
        public int? AssignedUserId { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public Models.TaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = default!;
        public int? AssignedUserId { get; set; }
        public string? AssignedUsername { get; set; }
        public int CommentCount { get; set; }
    }
}

