using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly TaskDbContext _db;
        private readonly IUserContext _userContext;

        public CommentService(TaskDbContext db, IUserContext userContext)
        {
            _db = db;
            _userContext = userContext;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByTaskAsync(int taskId, int userId)
        {
            // Verify task belongs to user's project
            var task = await _db.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null)
            {
                return Enumerable.Empty<CommentDto>();
            }

            var comments = await _db.Comments
                .Include(c => c.TaskItem)
                .Include(c => c.User)
                .Where(c => c.TaskItemId == taskId)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();

            return comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Text = c.Text,
                CreatedDate = c.CreatedDate,
                TaskItemId = c.TaskItemId,
                TaskTitle = c.TaskItem.Title,
                UserId = c.UserId,
                Username = c.User.Username
            });
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int commentId, int userId)
        {
            var comment = await _db.Comments
                .Include(c => c.TaskItem)
                .ThenInclude(t => t.Project)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.TaskItem.Project.UserId == userId);

            if (comment == null) return null;

            return new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedDate = comment.CreatedDate,
                TaskItemId = comment.TaskItemId,
                TaskTitle = comment.TaskItem.Title,
                UserId = comment.UserId,
                Username = comment.User.Username
            };
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, int userId)
        {
            // Verify task belongs to user's project
            var task = await _db.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == dto.TaskItemId && t.Project.UserId == userId);

            if (task == null)
            {
                throw new UnauthorizedAccessException("Task not found or you don't have access to it.");
            }

            var comment = new Models.Comment
            {
                Text = dto.Text.Trim(),
                CreatedDate = DateTime.UtcNow,
                TaskItemId = dto.TaskItemId,
                UserId = userId
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // Reload with navigation properties
            await _db.Entry(comment)
                .Reference(c => c.TaskItem)
                .LoadAsync();
            await _db.Entry(comment)
                .Reference(c => c.User)
                .LoadAsync();

            return new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedDate = comment.CreatedDate,
                TaskItemId = comment.TaskItemId,
                TaskTitle = comment.TaskItem.Title,
                UserId = comment.UserId,
                Username = comment.User.Username
            };
        }

        public async Task<CommentDto?> UpdateCommentAsync(int commentId, UpdateCommentDto dto, int userId)
        {
            var comment = await _db.Comments
                .Include(c => c.TaskItem)
                .ThenInclude(t => t.Project)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.TaskItem.Project.UserId == userId);

            if (comment == null) return null;

            // Only the comment owner can update
            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only update your own comments.");
            }

            comment.Text = dto.Text.Trim();
            await _db.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedDate = comment.CreatedDate,
                TaskItemId = comment.TaskItemId,
                TaskTitle = comment.TaskItem.Title,
                UserId = comment.UserId,
                Username = comment.User.Username
            };
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _db.Comments
                .Include(c => c.TaskItem)
                .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.TaskItem.Project.UserId == userId);

            if (comment == null) return false;

            // Only the comment owner can delete
            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own comments.");
            }

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}

