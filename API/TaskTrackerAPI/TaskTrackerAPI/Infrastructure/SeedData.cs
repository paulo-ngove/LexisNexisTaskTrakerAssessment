using TaskTrackerAPI.Domain.Enums;
using TaskTrackerAPI.Domain.Models;

namespace TaskTrackerAPI.Infrastructure
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (!context.Tasks.Any())
            {
                var tasks = new List<TaskItem>
                {
                    new TaskItem
                    {
                        Title = "Complete API Implementation",
                        Description = "Finish building the RESTful API for tasks",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        DueDate = DateTime.UtcNow.AddDays(1),
                        Priority = Priority.High
                    },
                    new TaskItem
                    {
                        Title = "Write Documentation",
                        Description = "Create API documentation for the task system",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        DueDate = DateTime.UtcNow.AddDays(3),
                        Priority = Priority.Medium
                    },
                    new TaskItem
                    {
                        Title = "Test API Endpoints",
                        Description = "Write unit tests for all API endpoints",
                        CreatedAt = DateTime.UtcNow,
                        DueDate = DateTime.UtcNow.AddDays(5),
                        Priority = Priority.High
                    },
                    new TaskItem
                    {
                        Title = "Setup CI/CD Pipeline",
                        Description = "Configure continuous integration and deployment",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        DueDate = DateTime.UtcNow.AddDays(7),
                        Priority = Priority.Low
                    },
                    new TaskItem
                    {
                        Title = "Design Frontend UI",
                        Description = "Create mockups for the SPA frontend",
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        DueDate = DateTime.UtcNow.AddDays(-1),
                        Priority = Priority.Medium
                    }
                };

                context.Tasks.AddRange(tasks);
                context.SaveChanges();
            }
        }
    }
}
