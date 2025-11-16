using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _db;
        private readonly IUserContext _userContext;

        public TaskService(TaskDbContext db, IUserContext userContext)
        {
            _db = db;
            _userContext = userContext;
        }

        public async Task<IEnumerable<TaskDto>> GetTasksAsync(int userId, int? projectId = null, Models.TaskStatus? status = null)
        {
            var query = _db.TaskItems
                .Include(t => t.Project)
                .ThenInclude(p => p.User)
                .Include(t => t.AssignedUser)
                .Include(t => t.Comments)
                .Where(t => t.Project.UserId == userId) // Only tasks from user's projects
                .AsQueryable();

            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value && t.Project.UserId == userId);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            var tasks = await query.ToListAsync();

            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                AssignedUserId = t.AssignedUserId,
                AssignedUsername = t.AssignedUser?.Username,
                CommentCount = t.Comments.Count
            });
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId, int userId)
        {
            var task = await _db.TaskItems
                .Include(t => t.Project)
                .ThenInclude(p => p.User)
                .Include(t => t.AssignedUser)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null) return null;

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedUserId = task.AssignedUserId,
                AssignedUsername = task.AssignedUser?.Username,
                CommentCount = task.Comments.Count
            };
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int userId)
        {
            // Verify project ownership
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.UserId == userId);

            if (project == null)
            {
                throw new UnauthorizedAccessException("Project not found or you don't have access to it.");
            }

            // Verify assigned user exists if provided
            if (dto.AssignedUserId.HasValue)
            {
                var assignedUser = await _db.Users.FindAsync(dto.AssignedUserId.Value);
                if (assignedUser == null)
                {
                    throw new ArgumentException("Assigned user not found.");
                }
            }

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Status = dto.Status,
                DueDate = dto.DueDate,
                ProjectId = dto.ProjectId,
                AssignedUserId = dto.AssignedUserId
            };

            _db.TaskItems.Add(task);
            await _db.SaveChangesAsync();

            // Reload with navigation properties
            await _db.Entry(task)
                .Reference(t => t.Project)
                .LoadAsync();
            await _db.Entry(task.Project)
                .Reference(p => p.User)
                .LoadAsync();
            await _db.Entry(task)
                .Reference(t => t.AssignedUser)
                .LoadAsync();
            await _db.Entry(task)
                .Collection(t => t.Comments)
                .LoadAsync();

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedUserId = task.AssignedUserId,
                AssignedUsername = task.AssignedUser?.Username,
                CommentCount = task.Comments.Count
            };
        }

        public async Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto dto, int userId)
        {
            var task = await _db.TaskItems
                .Include(t => t.Project)
                .ThenInclude(p => p.User)
                .Include(t => t.AssignedUser)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null) return null;

            // Verify new project ownership if changed
            if (task.ProjectId != dto.ProjectId)
            {
                var newProject = await _db.Projects
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.UserId == userId);

                if (newProject == null)
                {
                    throw new UnauthorizedAccessException("Project not found or you don't have access to it.");
                }
            }

            // Verify assigned user exists if provided
            if (dto.AssignedUserId.HasValue)
            {
                var assignedUser = await _db.Users.FindAsync(dto.AssignedUserId.Value);
                if (assignedUser == null)
                {
                    throw new ArgumentException("Assigned user not found.");
                }
            }

            task.Title = dto.Title.Trim();
            task.Description = dto.Description?.Trim() ?? string.Empty;
            task.Status = dto.Status;
            task.DueDate = dto.DueDate;
            task.ProjectId = dto.ProjectId;
            task.AssignedUserId = dto.AssignedUserId;

            await _db.SaveChangesAsync();

            // Reload navigation properties
            await _db.Entry(task)
                .Reference(t => t.Project)
                .LoadAsync();
            await _db.Entry(task.Project)
                .Reference(p => p.User)
                .LoadAsync();
            await _db.Entry(task)
                .Reference(t => t.AssignedUser)
                .LoadAsync();

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedUserId = task.AssignedUserId,
                AssignedUsername = task.AssignedUser?.Username,
                CommentCount = task.Comments.Count
            };
        }

        public async Task<TaskDto?> UpdateTaskStatusAsync(int taskId, Models.TaskStatus status, int userId)
        {
            var task = await _db.TaskItems
                .Include(t => t.Project)
                .ThenInclude(p => p.User)
                .Include(t => t.AssignedUser)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null) return null;

            task.Status = status;
            await _db.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedUserId = task.AssignedUserId,
                AssignedUsername = task.AssignedUser?.Username,
                CommentCount = task.Comments.Count
            };
        }

        public async Task<TaskDto?> AssignTaskAsync(int taskId, int? assignedUserId, int userId)
        {
            var task = await _db.TaskItems
                .Include(t => t.Project)
                .ThenInclude(p => p.User)
                .Include(t => t.AssignedUser)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null) return null;

            // Verify assigned user exists if provided
            if (assignedUserId.HasValue)
            {
                var assignedUser = await _db.Users.FindAsync(assignedUserId.Value);
                if (assignedUser == null)
                {
                    throw new ArgumentException("Assigned user not found.");
                }
            }

            task.AssignedUserId = assignedUserId;
            await _db.SaveChangesAsync();

            // Reload assigned user
            if (assignedUserId.HasValue)
            {
                await _db.Entry(task)
                    .Reference(t => t.AssignedUser)
                    .LoadAsync();
            }

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedUserId = task.AssignedUserId,
                AssignedUsername = task.AssignedUser?.Username,
                CommentCount = task.Comments.Count
            };
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await _db.TaskItems
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null) return false;

            _db.TaskItems.Remove(task);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}

