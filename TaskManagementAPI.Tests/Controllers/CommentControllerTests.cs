using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using TaskManagementAPI.Tests.Helpers;

namespace TaskManagementAPI.Tests.Controllers
{
    public class CommentControllerTests
    {
        private TaskDbContext _db;
        private ICommentService _commentService;
        private IUserContext _userContext;
        private CommentController _controller;

        public CommentControllerTests()
        {
            _db = TestDbContextFactory.CreateInMemoryContext();
            _userContext = new FakeUserContext(1, "alice");
            _commentService = new CommentService(_db, _userContext);
            _controller = new CommentController(_commentService, _userContext);
        }

        [Fact]
        public async Task GetCommentsByTask_ReturnsOk_WithTaskComments()
        {
            // Act
            var result = await _controller.GetCommentsByTask(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comments = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(okResult.Value);
            Assert.Single(comments); // Comment 1 belongs to task 1
        }

        [Fact]
        public async Task GetCommentsByTask_ReturnsEmpty_WhenTaskNotAccessible()
        {
            // Act
            var result = await _controller.GetCommentsByTask(2); // Task belongs to project 2

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comments = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(okResult.Value);
            Assert.Empty(comments);
        }

        [Fact]
        public async Task GetComment_ReturnsOk_WhenCommentExistsAndAccessible()
        {
            // Act
            var result = await _controller.GetComment(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comment = Assert.IsType<CommentDto>(okResult.Value);
            Assert.Equal(1, comment.Id);
        }

        [Fact]
        public async Task GetComment_ReturnsNotFound_WhenCommentNotAccessible()
        {
            // Act - Comment 1 is accessible, but let's try a non-existent comment
            var result = await _controller.GetComment(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_ReturnsCreated_WithValidData()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                Text = "New Comment",
                TaskItemId = 1 // Task belongs to project 1 (owned by user 1)
            };

            // Act
            var result = await _controller.CreateComment(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var comment = Assert.IsType<CommentDto>(createdResult.Value);
            Assert.Equal("New Comment", comment.Text);
        }

        [Fact]
        public async Task CreateComment_ReturnsNotFound_WhenTaskNotAccessible()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                Text = "New Comment",
                TaskItemId = 2 // Task belongs to project 2 (owned by user 2)
            };

            // Act
            var result = await _controller.CreateComment(dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateComment_ReturnsOk_WhenCommentOwned()
        {
            // Arrange
            var dto = new UpdateCommentDto
            {
                Text = "Updated Comment"
            };

            // Act
            var result = await _controller.UpdateComment(1, dto); // Comment 1 is owned by user 1

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comment = Assert.IsType<CommentDto>(okResult.Value);
            Assert.Equal("Updated Comment", comment.Text);
        }

        [Fact]
        public async Task DeleteComment_ReturnsNoContent_WhenCommentOwned()
        {
            // Act
            var result = await _controller.DeleteComment(1); // Comment 1 is owned by user 1

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteComment_ReturnsNotFound_WhenCommentNotExists()
        {
            // Act
            var result = await _controller.DeleteComment(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #region Additional Tests for ~100% Coverage

        [Fact]
        public async Task GetCommentsByTask_ReturnsEmptyList_WhenTaskHasNoComments()
        {
            // Arrange - Task 2 has a comment, but user 1 can't access it
            // Let's use a task with no comments - we'll create one for project 1
            var newTask = new Models.TaskItem
            {
                Title = "Task without comments",
                Description = "No comments here",
                Status = Models.TaskStatus.ToDo,
                ProjectId = 1,
                DueDate = DateTime.UtcNow.AddDays(3)
            };
            _db.TaskItems.Add(newTask);
            await _db.SaveChangesAsync();

            // Act
            var result = await _controller.GetCommentsByTask(newTask.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comments = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(okResult.Value);
            Assert.Empty(comments);
        }

        [Fact]
        public async Task GetComment_ReturnsNotFound_WhenCommentBelongsToOtherUsersTask()
        {
            // Act - Comment 2 belongs to task 2 (user 2's project)
            var result = await _controller.GetComment(2);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_ReturnsBadRequest_WithEmptyText()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                Text = "",
                TaskItemId = 1
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Text", "The Text field is required.");

            // Act
            var result = await _controller.CreateComment(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_ReturnsBadRequest_WithTooLongText()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                Text = new string('A', 1001), // 1001 characters (max is 1000)
                TaskItemId = 1
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Text", "The field Text must be a string with a maximum length of 1000.");

            // Act
            var result = await _controller.CreateComment(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateComment_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                Text = "Comment on non-existent task",
                TaskItemId = 999 // Non-existent task
            };

            // Act
            var result = await _controller.CreateComment(dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateComment_ReturnsNotFound_WhenCommentNotOwned()
        {
            // Arrange - Comment 2 belongs to user 2
            var dto = new UpdateCommentDto
            {
                Text = "Trying to update another user's comment"
            };

            // Act
            var result = await _controller.UpdateComment(2, dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateComment_ReturnsBadRequest_WithEmptyText()
        {
            // Arrange
            var dto = new UpdateCommentDto
            {
                Text = ""
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Text", "The Text field is required.");

            // Act
            var result = await _controller.UpdateComment(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteComment_ReturnsNotFound_WhenCommentBelongsToOtherUser()
        {
            // Act - Comment 2 belongs to user 2
            var result = await _controller.DeleteComment(2);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion
    }
}


