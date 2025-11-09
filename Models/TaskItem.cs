using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; }
        
        public TaskStatus Status { get; set; }
        
        public DateTime DueDate { get; set; }
        
        // Foreign keys
        public int ProjectId { get; set; }
        public int? AssignedUserId { get; set; }
        
        // Navigation properties
        public Project Project { get; set; }
        public User AssignedUser { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
    
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}