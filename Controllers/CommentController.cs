using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IUserContext _userContext;

        public CommentController(ICommentService commentService, IUserContext userContext)
        {
            _commentService = commentService;
            _userContext = userContext;
        }

        /// <summary>
        /// Get all comments for a specific task (only if task belongs to user's projects)
        /// </summary>
        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByTask(int taskId)
        {
            var comments = await _commentService.GetCommentsByTaskAsync(taskId, _userContext.UserId);
            return Ok(comments);
        }

        /// <summary>
        /// Get a specific comment by ID (only if it belongs to a task in user's projects)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id, _userContext.UserId);
            if (comment == null)
            {
                return NotFound("Comment not found or you don't have access to it.");
            }
            return Ok(comment);
        }

        /// <summary>
        /// Create a new comment on a task (only if task belongs to user's projects)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var comment = await _commentService.CreateCommentAsync(dto, _userContext.UserId);
                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing comment (only if comment owner and task belongs to user's projects)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(int id, [FromBody] UpdateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var comment = await _commentService.UpdateCommentAsync(id, dto, _userContext.UserId);
                if (comment == null)
                {
                    return NotFound("Comment not found or you don't have access to it.");
                }
                return Ok(comment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        /// <summary>
        /// Delete a comment (only if comment owner and task belongs to user's projects)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var deleted = await _commentService.DeleteCommentAsync(id, _userContext.UserId);
                if (!deleted)
                {
                    return NotFound("Comment not found or you don't have access to it.");
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}

