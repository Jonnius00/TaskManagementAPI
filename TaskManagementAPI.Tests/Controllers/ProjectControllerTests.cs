using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
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
    }
}

