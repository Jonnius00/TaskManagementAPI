using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsAsync(int userId, string? search = null);
        Task<ProjectDto?> GetProjectByIdAsync(int projectId, int userId);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, int userId);
        Task<ProjectDto?> UpdateProjectAsync(int projectId, UpdateProjectDto dto, int userId);
        Task<bool> DeleteProjectAsync(int projectId, int userId);
    }
}

