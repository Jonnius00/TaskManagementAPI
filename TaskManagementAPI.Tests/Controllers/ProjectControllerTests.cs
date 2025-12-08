using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using TaskManagementAPI.Tests.Helpers;

namespace TaskManagementAPI.Tests.Controllers
{
    public class ProjectControllerTests
    {
        private TaskDbContext _db;
        private IProjectService _projectService;
        private IUserContext _userContext;
        private ProjectController _controller;

        public ProjectControllerTests()
        {
            _db = TestDbContextFactory.CreateInMemoryContext();
            _userContext = new FakeUserContext(1, "alice");
            _projectService = new ProjectService(_db, _userContext);
            _controller = new ProjectController(_projectService, _userContext);
        }

        [Fact]
        public async Task GetProjects_ReturnsOk_WithUserProjects()
        {
            // Act
            var result = await _controller.GetProjects();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var projects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
            Assert.Single(projects); // Only project 1 (owned by user 1)
        }

        [Fact]
        public async Task GetProjects_WithSearch_ReturnsFilteredProjects()
        {
            // Act
            var result = await _controller.GetProjects("Project 1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var projects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
            Assert.Single(projects);
            Assert.Equal("Project 1", projects.First().Name);
        }

        [Fact]
        public async Task GetProject_ReturnsOk_WhenProjectExistsAndOwned()
        {
            // Act
            var result = await _controller.GetProject(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var project = Assert.IsType<ProjectDto>(okResult.Value);
            Assert.Equal(1, project.Id);
            Assert.Equal("Project 1", project.Name);
        }

        [Fact]
        public async Task GetProject_ReturnsNotFound_WhenProjectNotOwned()
        {
            // Act
            var result = await _controller.GetProject(2); // Project owned by user 2

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateProject_ReturnsCreated_WithValidData()
        {
            // Arrange
            var dto = new CreateProjectDto
            {
                Name = "New Project",
                Description = "New Description"
            };

            // Act
            var result = await _controller.CreateProject(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var project = Assert.IsType<ProjectDto>(createdResult.Value);
            Assert.Equal("New Project", project.Name);
        }

        [Fact]
        public async Task UpdateProject_ReturnsOk_WhenProjectExistsAndOwned()
        {
            // Arrange
            var dto = new UpdateProjectDto
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };

            // Act
            var result = await _controller.UpdateProject(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var project = Assert.IsType<ProjectDto>(okResult.Value);
            Assert.Equal("Updated Project", project.Name);
        }

        [Fact]
        public async Task UpdateProject_ReturnsNotFound_WhenProjectNotOwned()
        {
            // Arrange
            var dto = new UpdateProjectDto
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };

            // Act
            var result = await _controller.UpdateProject(2, dto); // Project owned by user 2

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteProject_ReturnsNoContent_WhenProjectExistsAndOwned()
        {
            // Act
            var result = await _controller.DeleteProject(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProject_ReturnsNotFound_WhenProjectNotOwned()
        {
            // Act
            var result = await _controller.DeleteProject(2); // Project owned by user 2

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #region Additional Tests for 100% Coverage

        [Fact]
        public async Task GetProjects_ReturnsEmptyList_WhenUserHasNoProjects()
        {
            // Arrange - Create a new controller with user 3 who has no projects
            var userContext = new FakeUserContext(3, "charlie");
            var projectService = new ProjectService(_db, userContext);
            var controller = new ProjectController(projectService, userContext);

            // Act
            var result = await controller.GetProjects();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var projects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
            Assert.Empty(projects);
        }

        [Fact]
        public async Task GetProjects_WithSearch_ReturnsEmptyList_WhenNoMatches()
        {
            // Act
            var result = await _controller.GetProjects("NonExistentProject");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var projects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
            Assert.Empty(projects);
        }

        [Fact]
        public async Task GetProject_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            // Act
            var result = await _controller.GetProject(999); // Non-existent project ID

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateProject_ReturnsBadRequest_WithEmptyName()
        {
            // Arrange
            var dto = new CreateProjectDto
            {
                Name = "",
                Description = "Valid Description"
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.CreateProject(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateProject_ReturnsBadRequest_WithTooLongName()
        {
            // Arrange
            var dto = new CreateProjectDto
            {
                Name = new string('A', 101), // 101 characters (max is 100)
                Description = "Valid Description"
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Name", "The field Name must be a string with a maximum length of 100.");

            // Act
            var result = await _controller.CreateProject(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProject_ReturnsBadRequest_WithEmptyName()
        {
            // Arrange
            var dto = new UpdateProjectDto
            {
                Name = "",
                Description = "Valid Description"
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.UpdateProject(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProject_ReturnsBadRequest_WithTooLongDescription()
        {
            // Arrange
            var dto = new UpdateProjectDto
            {
                Name = "Valid Name",
                Description = new string('B', 501) // 501 characters (max is 500)
            };

            // Manually trigger model validation
            _controller.ModelState.AddModelError("Description", "The field Description must be a string with a maximum length of 500.");

            // Act
            var result = await _controller.UpdateProject(1, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteProject_WithTasks_DeletesProjectAndTasks()
        {
            // Arrange - Add a task to project 1
            var task = new TaskItem
            {
                Title = "Task for deletion test",
                Description = "This task should be deleted with the project",
                Status = Models.TaskStatus.ToDo,
                ProjectId = 1,
                DueDate = DateTime.UtcNow.AddDays(7)
            };
            _db.TaskItems.Add(task);
            await _db.SaveChangesAsync();

            var taskId = task.Id;

            // Act - Delete the project
            var result = await _controller.DeleteProject(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify project is deleted
            var deletedProject = await _db.Projects.FindAsync(1);
            Assert.Null(deletedProject);

            // Verify task is also deleted (cascade delete)
            var deletedTask = await _db.TaskItems.FindAsync(taskId);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task CreateProject_SetsCorrectUserId()
        {
            // Arrange
            var dto = new CreateProjectDto
            {
                Name = "Ownership Test Project",
                Description = "Testing user ownership"
            };

            // Act
            var result = await _controller.CreateProject(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var project = Assert.IsType<ProjectDto>(createdResult.Value);
            Assert.Equal(1, project.UserId); // Should be owned by user 1 (alice)
            Assert.Equal("alice", project.OwnerUsername);
        }

        #endregion
    }
}

