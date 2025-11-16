using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserContext _userContext;

        public ProjectController(IProjectService projectService, IUserContext userContext)
        {
            _projectService = projectService;
            _userContext = userContext;
        }

        /// <summary>
        /// Get all projects for the current user, optionally filtered by search term
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects([FromQuery] string? search = null)
        {
            var projects = await _projectService.GetProjectsAsync(_userContext.UserId, search);
            return Ok(projects);
        }

        /// <summary>
        /// Get a specific project by ID (only if owned by current user)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id, _userContext.UserId);
            if (project == null)
            {
                return NotFound("Project not found or you don't have access to it.");
            }
            return Ok(project);
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _projectService.CreateProjectAsync(dto, _userContext.UserId);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        /// <summary>
        /// Update an existing project (only if owned by current user)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject(int id, [FromBody] UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _projectService.UpdateProjectAsync(id, dto, _userContext.UserId);
            if (project == null)
            {
                return NotFound("Project not found or you don't have access to it.");
            }
            return Ok(project);
        }

        /// <summary>
        /// Delete a project (only if owned by current user)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var deleted = await _projectService.DeleteProjectAsync(id, _userContext.UserId);
            if (!deleted)
            {
                return NotFound("Project not found or you don't have access to it.");
            }
            return NoContent();
        }
    }
}

