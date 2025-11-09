using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class User
    {
        public User()
        {
            Projects = new List<Project>();
            AssignedTasks = new List<TaskItem>();
            Comments = new List<Comment>();
        }

        public User(string username, string email, string passwordHash) : this()
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }

        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        // Navigation properties
        public ICollection<Project> Projects { get; set; }
        public ICollection<TaskItem> AssignedTasks { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}