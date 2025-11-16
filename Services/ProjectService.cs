using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly TaskDbContext _db;
        private readonly IUserContext _userContext;

        public ProjectService(TaskDbContext db, IUserContext userContext)
        {
            _db = db;
            _userContext = userContext;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(int userId, string? search = null)
        {
            var query = _db.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                .Include(p => p.Tasks)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => 
                    p.Name.Contains(search) || 
                    p.Description.Contains(search));
            }

            var projects = await query.ToListAsync();

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedDate = p.CreatedDate,
                UserId = p.UserId,
                OwnerUsername = p.User.Username,
                TaskCount = p.Tasks.Count
            });
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int projectId, int userId)
        {
            var project = await _db.Projects
                .Include(p => p.User)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null) return null;

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedDate = project.CreatedDate,
                UserId = project.UserId,
                OwnerUsername = project.User.Username,
                TaskCount = project.Tasks.Count
            };
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, int userId)
        {
            var project = new Project
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                CreatedDate = DateTime.UtcNow,
                UserId = userId
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            // Reload with navigation properties
            await _db.Entry(project)
                .Reference(p => p.User)
                .LoadAsync();
            await _db.Entry(project)
                .Collection(p => p.Tasks)
                .LoadAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedDate = project.CreatedDate,
                UserId = project.UserId,
                OwnerUsername = project.User.Username,
                TaskCount = project.Tasks.Count
            };
        }

        public async Task<ProjectDto?> UpdateProjectAsync(int projectId, UpdateProjectDto dto, int userId)
        {
            var project = await _db.Projects
                .Include(p => p.User)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null) return null;

            project.Name = dto.Name.Trim();
            project.Description = dto.Description?.Trim() ?? string.Empty;

            await _db.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedDate = project.CreatedDate,
                UserId = project.UserId,
                OwnerUsername = project.User.Username,
                TaskCount = project.Tasks.Count
            };
        }

        public async Task<bool> DeleteProjectAsync(int projectId, int userId)
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null) return false;

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}

