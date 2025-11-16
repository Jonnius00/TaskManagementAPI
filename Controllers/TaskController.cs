using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IUserContext _userContext;

        public TaskController(ITaskService taskService, IUserContext userContext)
        {
            _taskService = taskService;
            _userContext = userContext;
        }

        /// <summary>
        /// Get all tasks for the current user's projects, optionally filtered by projectId and status
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(
            [FromQuery] int? projectId = null,
            [FromQuery] Models.TaskStatus? status = null)
        {
            var tasks = await _taskService.GetTasksAsync(_userContext.UserId, projectId, status);
            return Ok(tasks);
        }

        /// <summary>
        /// Get a specific task by ID (only if it belongs to user's projects)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, _userContext.UserId);
            if (task == null)
            {
                return NotFound("Task not found or you don't have access to it.");
            }
            return Ok(task);
        }

        /// <summary>
        /// Create a new task in one of the user's projects
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var task = await _taskService.CreateTaskAsync(dto, _userContext.UserId);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing task (only if it belongs to user's projects)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var task = await _taskService.UpdateTaskAsync(id, dto, _userContext.UserId);
                if (task == null)
                {
                    return NotFound("Task not found or you don't have access to it.");
                }
                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update the status of a task (only if it belongs to user's projects)
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<TaskDto>> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _taskService.UpdateTaskStatusAsync(id, dto.Status, _userContext.UserId);
            if (task == null)
            {
                return NotFound("Task not found or you don't have access to it.");
            }
            return Ok(task);
        }

        /// <summary>
        /// Assign a task to a user (only if task belongs to user's projects)
        /// </summary>
        [HttpPatch("{id}/assign")]
        public async Task<ActionResult<TaskDto>> AssignTask(int id, [FromBody] AssignTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var task = await _taskService.AssignTaskAsync(id, dto.AssignedUserId, _userContext.UserId);
                if (task == null)
                {
                    return NotFound("Task not found or you don't have access to it.");
                }
                return Ok(task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a task (only if it belongs to user's projects)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id, _userContext.UserId);
            if (!deleted)
            {
                return NotFound("Task not found or you don't have access to it.");
            }
            return NoContent();
        }
    }
}

