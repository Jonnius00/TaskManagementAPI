using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static TaskDbContext CreateInMemoryContext(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<TaskDbContext>()
                .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new TaskDbContext(options);
            SeedTestData(context);
            return context;
        }

        private static void SeedTestData(TaskDbContext context)
        {
            // Add test users
            var user1 = new User
            {
                Id = 1,
                Username = "alice",
                Email = "alice@test.com",
                PasswordHash = "hash1"
            };

            var user2 = new User
            {
                Id = 2,
                Username = "bob",
                Email = "bob@test.com",
                PasswordHash = "hash2"
            };

            context.Users.AddRange(user1, user2);

            // Add test projects
            var project1 = new Project
            {
                Id = 1,
                Name = "Project 1",
                Description = "Description 1",
                CreatedDate = DateTime.UtcNow,
                UserId = 1 // owned by alice
            };

            var project2 = new Project
            {
                Id = 2,
                Name = "Project 2",
                Description = "Description 2",
                CreatedDate = DateTime.UtcNow,
                UserId = 2 // owned by bob
            };

            context.Projects.AddRange(project1, project2);

            // Add test tasks
            var task1 = new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "Task Description 1",
                    Status = Models.TaskStatus.ToDo,
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = 1, // belongs to project 1
                AssignedUserId = 1
            };

            var task2 = new TaskItem
            {
                Id = 2,
                Title = "Task 2",
                Description = "Task Description 2",
                    Status = Models.TaskStatus.InProgress,
                DueDate = DateTime.UtcNow.AddDays(14),
                ProjectId = 2, // belongs to project 2
                AssignedUserId = 2
            };

            context.TaskItems.AddRange(task1, task2);

            // Add test comments
            var comment1 = new Comment
            {
                Id = 1,
                Text = "Comment 1",
                CreatedDate = DateTime.UtcNow,
                TaskItemId = 1,
                UserId = 1
            };

            context.Comments.Add(comment1);

            context.SaveChanges();
        }
    }
}

