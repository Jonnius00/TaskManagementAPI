using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        // Foreign key
        public int UserId { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}