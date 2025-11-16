using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        [StringLength(1000)]
        public string Text { get; set; } = default!;

        [Required]
        public int TaskItemId { get; set; }
    }

    public class UpdateCommentDto
    {
        [Required]
        [StringLength(1000)]
        public string Text { get; set; } = default!;
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public int TaskItemId { get; set; }
        public string TaskTitle { get; set; } = default!;
        public int UserId { get; set; }
        public string Username { get; set; } = default!;
    }
}

