using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class Comment
    {
        public Comment()
        {
        }

        public Comment(string text) : this()
        {
            Text = text;
        }

        public int Id { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Text { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        // Foreign keys
        public int TaskItemId { get; set; }
        public int UserId { get; set; }
        
        // Navigation properties
        public TaskItem TaskItem { get; set; }
        public User User { get; set; }
    }
}