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
    }
}

