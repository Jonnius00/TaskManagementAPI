using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetCommentsByTaskAsync(int taskId, int userId);
        Task<CommentDto?> GetCommentByIdAsync(int commentId, int userId);
        Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, int userId);
        Task<CommentDto?> UpdateCommentAsync(int commentId, UpdateCommentDto dto, int userId);
        Task<bool> DeleteCommentAsync(int commentId, int userId);
    }
}

