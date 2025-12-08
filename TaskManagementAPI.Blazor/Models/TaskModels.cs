namespace TaskManagementAPI.Blazor.Models
{
    /// <summary>
    /// Task status enumeration
    /// </summary>
    public enum TaskStatus
    {
        ToDo = 0,
        InProgress = 1,
        Done = 2
    }

    /// <summary>
    /// Task Data Transfer Object - Response from API
    /// </summary>
    public class TaskDto
    {
        /// <summary>
        /// Task unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Task title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed task description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Task status (0=ToDo, 1=InProgress, 2=Done)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Task creation date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Task due date (optional)
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Associated project ID
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Associated project name
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// User ID who created the task
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        /// Username of task creator
        /// </summary>
        public string CreatedByUsername { get; set; } = string.Empty;

        /// <summary>
        /// User ID assigned to this task (optional)
        /// </summary>
        public int? AssignedUserId { get; set; }

        /// <summary>
        /// Username of assigned user
        /// </summary>
        public string? AssignedUsername { get; set; }

        /// <summary>
        /// Human-readable status string
        /// </summary>
        public string StatusName => ((TaskStatus)Status).ToString();
    }

    /// <summary>
    /// Create Task Request DTO
    /// </summary>
    public class CreateTaskDto
    {
        /// <summary>
        /// Task title (required)
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Task description (optional)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Task status (default: 0 = ToDo)
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// Task due date (optional)
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Associated project ID (required)
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// User to assign task to (optional)
        /// </summary>
        public int? AssignedUserId { get; set; }
    }

    /// <summary>
    /// Update Task Request DTO
    /// </summary>
    public class UpdateTaskDto
    {
        /// <summary>
        /// Task title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Task description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Task status (0=ToDo, 1=InProgress, 2=Done)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Task due date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Associated project ID
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// User to assign task to
        /// </summary>
        public int? AssignedUserId { get; set; }
    }

    /// <summary>
    /// Update task status request
    /// </summary>
    public class UpdateTaskStatusDto
    {
        /// <summary>
        /// New task status
        /// </summary>
        public int Status { get; set; }
    }

    /// <summary>
    /// Assign task to user request
    /// </summary>
    public class AssignTaskDto
    {
        /// <summary>
        /// User ID to assign task to
        /// </summary>
        public int? AssignedUserId { get; set; }
    }
}
