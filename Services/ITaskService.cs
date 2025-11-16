using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksAsync(int userId, int? projectId = null, Models.TaskStatus? status = null);
        Task<TaskDto?> GetTaskByIdAsync(int taskId, int userId);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int userId);
        Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto dto, int userId);
        Task<TaskDto?> UpdateTaskStatusAsync(int taskId, Models.TaskStatus status, int userId);
        Task<TaskDto?> AssignTaskAsync(int taskId, int? assignedUserId, int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
    }
}

