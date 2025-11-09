using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User-Project relationship (One-to-Many)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User-TaskItem relationship (One-to-Many)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Project-TaskItem relationship (One-to-Many)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TaskItem-Comment relationship (One-to-Many)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.TaskItem)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User-Comment relationship (One-to-Many)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Status enum to store as string
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<string>();

            // Seed data for Users
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "alice", 
                    Email = "alice@example.com", 
                    PasswordHash = "AQAAAAEAACcQAAAAEJWeVy2ZcRFo4Uz1G8YJsN076QfIumswKzDiP9YGY8WfVXcEdjTO1KDJwkTEjBuUjw==" // Temporary hash for "Password123!"
                },
                new User 
                { 
                    Id = 2, 
                    Username = "bob", 
                    Email = "bob@example.com", 
                    PasswordHash = "AQAAAAEAACcQAAAAEJWeVy2ZcRFo4Uz1G8YJsN076QfIumswKzDiP9YGY8WfVXcEdjTO1KDJwkTEjBuUjw==" // Temporary hash for "Password123!"
                }
            );

            // Seed data for Projects
            modelBuilder.Entity<Project>().HasData(
                new Project 
                { 
                    Id = 1, 
                    Name = "Sample Project 1", 
                    Description = "This is a sample project", 
                    CreatedDate = new DateTime(2025, 1, 1),
                    UserId = 1
                },
                new Project 
                { 
                    Id = 2, 
                    Name = "Sample Project 2", 
                    Description = "This is another sample project", 
                    CreatedDate = new DateTime(2025, 1, 1),
                    UserId = 2
                }
            );

            // Seed data for TaskItems
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem 
                { 
                    Id = 1, 
                    Title = "Sample Task 1", 
                    Description = "This is a sample task", 
                    Status = Models.TaskStatus.ToDo,
                    DueDate = new DateTime(2025, 1, 8),
                    ProjectId = 1,
                    AssignedUserId = 1
                },
                new TaskItem 
                { 
                    Id = 2, 
                    Title = "Sample Task 2", 
                    Description = "This is another sample task", 
                    Status = Models.TaskStatus.InProgress,
                    DueDate = new DateTime(2025, 1, 15),
                    ProjectId = 2,
                    AssignedUserId = 2
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}