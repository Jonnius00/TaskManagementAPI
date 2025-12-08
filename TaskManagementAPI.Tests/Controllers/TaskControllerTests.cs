using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using TaskManagementAPI.Tests.Helpers;

namespace TaskManagementAPI.Tests.Controllers
{
    public class TaskControllerTests
    {
        private TaskDbContext _db;
        private ITaskService _taskService;
        private IUserContext _userContext;
        private TaskController _controller;

        public TaskControllerTests()
        {
            _db = TestDbContextFactory.CreateInMemoryContext();
            _userContext = new FakeUserContext(1, "alice");
            _taskService = new TaskService(_db, _userContext);
            _controller = new TaskController(_taskService, _userContext);
        }

        [Fact]
        public async Task GetTasks_ReturnsOk_WithUserProjectTasks()
        {
            // Act
            var result = await _controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Single(tasks); // Only task 1 (belongs to project 1 owned by user 1)
        }

        [Fact]
        public async Task GetTasks_WithProjectId_ReturnsFilteredTasks()
        {
            // Act
            var result = await _controller.GetTasks(projectId: 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Single(tasks);
        }

        [Fact]
        public async Task GetTasks_WithStatus_ReturnsFilteredTasks()
        {
            // Act
            var result = await _controller.GetTasks(status: Models.TaskStatus.ToDo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Single(tasks);
            Assert.Equal(Models.TaskStatus.ToDo, tasks.First().Status);
        }

        [Fact]
        public async Task GetTask_ReturnsOk_WhenTaskExistsAndAccessible()
        {
            // Act
            var result = await _controller.GetTask(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var task = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal(1, task.Id);
        }

        [Fact]
        public async Task GetTask_ReturnsNotFound_WhenTaskNotAccessible()
        {
            // Act
            var result = await _controller.GetTask(2); // Task belongs to project 2 (owned by user 2)

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreated_WithValidData()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "New Task Description",
                Status = Models.TaskStatus.ToDo,
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = 1, // Project owned by user 1
                AssignedUserId = 1
            };

            // Act
            var result = await _controller.CreateTask(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var task = Assert.IsType<TaskDto>(createdResult.Value);
            Assert.Equal("New Task", task.Title);
        }

        [Fact]
        public async Task CreateTask_ReturnsNotFound_WhenProjectNotOwned()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "New Task Description",
                Status = Models.TaskStatus.ToDo,
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = 2, // Project owned by user 2
                AssignedUserId = 1
            };

            // Act
            var result = await _controller.CreateTask(dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTask_ReturnsOk_WhenTaskExistsAndAccessible()
        {
            // Arrange
            var dto = new UpdateTaskDto
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = Models.TaskStatus.InProgress,
                DueDate = DateTime.UtcNow.AddDays(10),
                ProjectId = 1,
                AssignedUserId = 1
            };

            // Act
            var result = await _controller.UpdateTask(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var task = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal("Updated Task", task.Title);
        }

        [Fact]
        public async Task UpdateTaskStatus_ReturnsOk_WhenTaskExistsAndAccessible()
        {
            // Arrange
            var dto = new UpdateTaskStatusDto
            {
                Status = Models.TaskStatus.Done
            };

            // Act
            var result = await _controller.UpdateTaskStatus(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var task = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal(Models.TaskStatus.Done, task.Status);
        }

        [Fact]
        public async Task AssignTask_ReturnsOk_WhenTaskExistsAndAccessible()
        {
            // Arrange
            var dto = new AssignTaskDto
            {
                AssignedUserId = 2
            };

            // Act
            var result = await _controller.AssignTask(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var task = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal(2, task.AssignedUserId);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNoContent_WhenTaskExistsAndAccessible()
        {
            // Act
            var result = await _controller.DeleteTask(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenTaskNotAccessible()
        {
            // Act
            var result = await _controller.DeleteTask(2); // Task belongs to project 2

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #region Additional Tests for ~100% Coverage

        [Fact]
        public async Task GetTasks_ReturnsEmptyList_WhenUserHasNoTasks()
        {
            // Arrange - Create a new controller with user 3 who has no tasks
            var userContext = new FakeUserContext(3, "charlie");
            var taskService = new TaskService(_db, userContext);
            var controller = new TaskController(taskService, userContext);

            // Act
            var result = await controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Empty(tasks);
        }

        [Fact]
        public async Task GetTasks_WithMultipleFilters_ReturnsCorrectTasks()
        {
            // Arrange - Add another ToDo task to project 1
            var newTask = new TaskItem
            {
                Title = "Another ToDo Task",
                Description = "Another task",
                Status = Models.TaskStatus.ToDo,
                ProjectId = 1,
                DueDate = DateTime.UtcNow.AddDays(5)
            };
            _db.TaskItems.Add(newTask);
            await _db.SaveChangesAsync();

            // Act - Filter by both project and status
            var result = await _controller.GetTasks(projectId: 1, status: Models.TaskStatus.ToDo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Equal(2, tasks.Count()); // Both ToDo tasks in project 1
            Assert.All(tasks, t => Assert.Equal(Models.TaskStatus.ToDo, t.Status));
        }

        [Fact]
        public async Task GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Act
            var result = await _controller.GetTask(999); // Non-existent task ID

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequest_WithEmptyTitle()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "",
                Description = "Valid Description",
                Status = Models.TaskStatus.ToDo,
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = 1
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.CreateTask(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequest_WithTooLongTitle()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = new string('A', 201), // 201 characters (max is 200)
                Description = "Valid Description",
                Status = Models.TaskStatus.ToDo,
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = 1
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Title", "The field Title must be a string with a maximum length of 200.");

            // Act
            var result = await _controller.CreateTask(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTask_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            var dto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "Description",
                Status = Models.TaskStatus.ToDo,
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = 999 // Non-existent project
            };

            // Act
            var result = await _controller.CreateTask(dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNotFound_WhenTaskNotAccessible()
        {
            // Arrange
            var dto = new UpdateTaskDto
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = Models.TaskStatus.InProgress,
                DueDate = DateTime.UtcNow.AddDays(10),
                ProjectId = 1,
                AssignedUserId = 1
            };

            // Act
            var result = await _controller.UpdateTask(2, dto); // Task 2 belongs to user 2's project

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTask_ReturnsBadRequest_WithEmptyTitle()
        {
            // Arrange
            var dto = new UpdateTaskDto
            {
                Title = "",
                Description = "Valid Description",
                Status = Models.TaskStatus.InProgress,
                DueDate = DateTime.UtcNow.AddDays(10),
                ProjectId = 1
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.UpdateTask(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNotFound_WhenMovingToNonExistentProject()
        {
            // Arrange
            var dto = new UpdateTaskDto
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = Models.TaskStatus.InProgress,
                DueDate = DateTime.UtcNow.AddDays(10),
                ProjectId = 999, // Non-existent project
                AssignedUserId = 1
            };

            // Act
            var result = await _controller.UpdateTask(1, dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTaskStatus_ReturnsNotFound_WhenTaskNotAccessible()
        {
            // Arrange
            var dto = new UpdateTaskStatusDto
            {
                Status = Models.TaskStatus.Done
            };

            // Act
            var result = await _controller.UpdateTaskStatus(2, dto); // Task 2 belongs to user 2

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTaskStatus_AllowsAllStatusTransitions()
        {
            // Arrange - Task 1 starts as ToDo
            var toInProgressDto = new UpdateTaskStatusDto { Status = Models.TaskStatus.InProgress };
            var toDoneDto = new UpdateTaskStatusDto { Status = Models.TaskStatus.Done };
            var backToInProgressDto = new UpdateTaskStatusDto { Status = Models.TaskStatus.InProgress };

            // Act & Assert - ToDo → InProgress
            var result1 = await _controller.UpdateTaskStatus(1, toInProgressDto);
            var okResult1 = Assert.IsType<OkObjectResult>(result1.Result);
            var task1 = Assert.IsType<TaskDto>(okResult1.Value);
            Assert.Equal(Models.TaskStatus.InProgress, task1.Status);

            // Act & Assert - InProgress → Done
            var result2 = await _controller.UpdateTaskStatus(1, toDoneDto);
            var okResult2 = Assert.IsType<OkObjectResult>(result2.Result);
            var task2 = Assert.IsType<TaskDto>(okResult2.Value);
            Assert.Equal(Models.TaskStatus.Done, task2.Status);

            // Act & Assert - Done → InProgress (backward transition)
            var result3 = await _controller.UpdateTaskStatus(1, backToInProgressDto);
            var okResult3 = Assert.IsType<OkObjectResult>(result3.Result);
            var task3 = Assert.IsType<TaskDto>(okResult3.Value);
            Assert.Equal(Models.TaskStatus.InProgress, task3.Status);
        }

        [Fact]
        public async Task AssignTask_ReturnsOk_WhenAssigningToSelf()
        {
            // Arrange - User 1 assigns task to themselves
            var dto = new AssignTaskDto
            {
                AssignedUserId = 1
            };

            // Act
            var result = await _controller.AssignTask(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var task = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal(1, task.AssignedUserId);
        }

        [Fact]
        public async Task AssignTask_ReturnsNotFound_WhenTaskNotAccessible()
        {
            // Arrange
            var dto = new AssignTaskDto
            {
                AssignedUserId = 1
            };

            // Act
            var result = await _controller.AssignTask(2, dto); // Task 2 belongs to user 2

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        #endregion
    }
}


