using TaskManagementAPI.Blazor.Models;

namespace TaskManagementAPI.Blazor.Services
{
    /// <summary>
    /// Service for project-related API communication
    /// </summary>
    public class ProjectApiService
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<ProjectApiService> _logger;

        public ProjectApiService(IApiClient apiClient, ILogger<ProjectApiService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Get all projects for the current user
        /// </summary>
        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all projects");
                var projects = await _apiClient.GetAsync<List<ProjectDto>>("/api/project");
                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch projects: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get projects filtered by search term
        /// </summary>
        public async Task<List<ProjectDto>> SearchProjectsAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation($"Searching projects with term: {searchTerm}");
                var endpoint = string.IsNullOrEmpty(searchTerm) 
                    ? "/api/project" 
                    : $"/api/project?search={Uri.EscapeDataString(searchTerm)}";
                
                var projects = await _apiClient.GetAsync<List<ProjectDto>>(endpoint);
                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to search projects: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get a specific project by ID
        /// </summary>
        public async Task<ProjectDto> GetProjectByIdAsync(int projectId)
        {
            try
            {
                _logger.LogInformation($"Fetching project {projectId}");
                var project = await _apiClient.GetAsync<ProjectDto>($"/api/project/{projectId}");
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch project {projectId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto)
        {
            try
            {
                _logger.LogInformation($"Creating project: {createDto.Name}");
                var project = await _apiClient.PostAsync<ProjectDto>("/api/project", createDto);
                _logger.LogInformation($"Project created successfully with ID: {project.Id}");
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create project: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        public async Task<ProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto updateDto)
        {
            try
            {
                _logger.LogInformation($"Updating project {projectId}");
                var project = await _apiClient.PutAsync<ProjectDto>($"/api/project/{projectId}", updateDto);
                _logger.LogInformation($"Project {projectId} updated successfully");
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update project {projectId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        public async Task DeleteProjectAsync(int projectId)
        {
            try
            {
                _logger.LogInformation($"Deleting project {projectId}");
                await _apiClient.DeleteAsync($"/api/project/{projectId}");
                _logger.LogInformation($"Project {projectId} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete project {projectId}: {ex.Message}");
                throw;
            }
        }
    }
}
