using TaskManagementAPI.Blazor.Models;
using TaskModels = TaskManagementAPI.Blazor.Models;

namespace TaskManagementAPI.Blazor.Services
{
    /// <summary>
    /// Service for communicating with Task API endpoints
    /// </summary>
    public class TaskApiService
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<TaskApiService> _logger;

        public TaskApiService(IApiClient apiClient, ILogger<TaskApiService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks for the current user
        /// </summary>
        public async Task<List<TaskDto>> GetTasksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all tasks");
                var tasks = await _apiClient.GetAsync<List<TaskDto>>("/api/task");
                return tasks ?? new List<TaskDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching tasks: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get tasks filtered by project ID
        /// </summary>
        public async Task<List<TaskDto>> GetTasksByProjectAsync(int projectId)
        {
            try
            {
                _logger.LogInformation($"Fetching tasks for project {projectId}");
                var tasks = await _apiClient.GetAsync<List<TaskDto>>($"/api/task?projectId={projectId}");
                return tasks ?? new List<TaskDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching tasks for project {projectId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get tasks filtered by status
        /// </summary>
        public async Task<List<TaskDto>> GetTasksByStatusAsync(TaskModels.TaskStatus status)
        {
            try
            {
                _logger.LogInformation($"Fetching tasks with status {status}");
                var tasks = await _apiClient.GetAsync<List<TaskDto>>($"/api/task?status={(int)status}");
                return tasks ?? new List<TaskDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching tasks with status {status}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get a single task by ID
        /// </summary>
        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching task {id}");
                return await _apiClient.GetAsync<TaskDto>($"/api/task/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching task {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto taskData)
        {
            try
            {
                _logger.LogInformation($"Creating task: {taskData.Title}");
                var createdTask = await _apiClient.PostAsync<TaskDto>("/api/task", taskData);
                _logger.LogInformation($"Task created with ID: {createdTask.Id}");
                return createdTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating task: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto taskData)
        {
            try
            {
                _logger.LogInformation($"Updating task {id}");
                var updatedTask = await _apiClient.PutAsync<TaskDto>($"/api/task/{id}", taskData);
                _logger.LogInformation($"Task {id} updated successfully");
                return updatedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating task {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update task status
        /// </summary>
        public async Task<TaskDto> UpdateTaskStatusAsync(int id, TaskModels.TaskStatus status)
        {
            try
            {
                _logger.LogInformation($"Updating task {id} status to {status}");
                var statusUpdate = new UpdateTaskStatusDto { Status = (int)status };
                var updatedTask = await _apiClient.PatchAsync<TaskDto>($"/api/task/{id}/status", statusUpdate);
                _logger.LogInformation($"Task {id} status updated to {status}");
                return updatedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating task {id} status: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Assign task to a user
        /// </summary>
        public async Task<TaskDto> AssignTaskAsync(int id, int? assignedUserId)
        {
            try
            {
                _logger.LogInformation($"Assigning task {id} to user {assignedUserId}");
                var assignData = new AssignTaskDto { AssignedUserId = assignedUserId };
                var updatedTask = await _apiClient.PatchAsync<TaskDto>($"/api/task/{id}/assign", assignData);
                _logger.LogInformation($"Task {id} assigned successfully");
                return updatedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error assigning task {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        public async Task DeleteTaskAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting task {id}");
                await _apiClient.DeleteAsync($"/api/task/{id}");
                _logger.LogInformation($"Task {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting task {id}: {ex.Message}");
                throw;
            }
        }
    }
}
