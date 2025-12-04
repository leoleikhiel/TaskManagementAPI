namespace TaskManagementAPI.Data
{
    using BCrypt.Net;
    using Microsoft.EntityFrameworkCore;
    using TaskManagementAPI.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DatabaseSeeder
    {
        public static async System.Threading.Tasks.Task SeedDatabaseAsync(ApplicationDbContext context)
        {
            // Only seed if database doesn't have data
            if (await context.Users.AnyAsync() || await context.Tasks.AnyAsync() || 
                await context.Categories.AnyAsync() || await context.TaskNotes.AnyAsync())
            {
                return;
            }

            // Clear all existing data to start fresh
            await ClearAllDataAsync(context);

            // Create users
            var users = CreateUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Create categories
            var categories = CreateCategories(users);
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Create tasks with various due date scenarios
            var tasks = CreateTasks(users, categories);
            await context.Tasks.AddRangeAsync(tasks);
            await context.SaveChangesAsync();

            // Create task notes
            var taskNotes = CreateTaskNotes(tasks);
            await context.TaskNotes.AddRangeAsync(taskNotes);
            await context.SaveChangesAsync();
        }

        private static async System.Threading.Tasks.Task ClearAllDataAsync(ApplicationDbContext context)
        {
            try
            {
                // Delete all TaskNotes first (due to foreign key constraint)
                var taskNotes = await context.TaskNotes.ToListAsync();
                if (taskNotes.Any())
                {
                    context.TaskNotes.RemoveRange(taskNotes);
                    await context.SaveChangesAsync();
                }

                // Delete all Tasks
                var tasks = await context.Tasks.ToListAsync();
                if (tasks.Any())
                {
                    context.Tasks.RemoveRange(tasks);
                    await context.SaveChangesAsync();
                }

                // Delete all Categories
                var categories = await context.Categories.ToListAsync();
                if (categories.Any())
                {
                    context.Categories.RemoveRange(categories);
                    await context.SaveChangesAsync();
                }

                // Delete all Users
                var users = await context.Users.ToListAsync();
                if (users.Any())
                {
                    context.Users.RemoveRange(users);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing data: {ex.Message}");
            }
        }

        private static List<User> CreateUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PasswordHash = HashPassword("Password123!"),
                    CreatedAt = DateTime.UtcNow,
                    UserRole = Role.Regular
                },
                new User
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    PasswordHash = HashPassword("Password456!"),
                    CreatedAt = DateTime.UtcNow,
                    UserRole = Role.Admin
                }
            };

            return users;
        }

        private static List<Category> CreateCategories(List<User> users)
        {
            var categories = new List<Category>();

            // Categories for John Doe
            var categoryNames = new[] { "Work", "Personal", "Shopping", "Health", "Education" };
            var today = DateTime.UtcNow;

            foreach (var categoryName in categoryNames)
            {
                categories.Add(new Category
                {
                    Name = categoryName,
                    UserId = users[0].Id // John Doe
                });
            }

            // Add some categories for Jane Smith
            var janeCategories = new[] { "Project A", "Project B", "Admin Tasks" };
            foreach (var categoryName in janeCategories)
            {
                categories.Add(new Category
                {
                    Name = categoryName,
                    UserId = users[1].Id // Jane Smith
                });
            }

            return categories;
        }

        private static List<Models.Task> CreateTasks(List<User> users, List<Category> categories)
        {
            var tasks = new List<Models.Task>();
            var today = DateTime.UtcNow.Date;
            var johnCategories = categories.Where(c => c.UserId == users[0].Id).ToList();
            var janeCategories = categories.Where(c => c.UserId == users[1].Id).ToList();

            // ============= TODAY TASKS =============
            tasks.Add(new Models.Task
            {
                Title = "Review quarterly report",
                Description = "Check and review the Q4 quarterly report",
                DueDate = today,
                ScheduledDate = today,
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-5)
            });

            tasks.Add(new Models.Task
            {
                Title = "Grocery shopping",
                Description = "Buy milk, eggs, and vegetables",
                DueDate = today,
                ScheduledDate = today,
                CategoryId = johnCategories[2].Id, // Shopping
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-3)
            });

            // ============= THIS WEEK TASKS (Due within 7 days) =============
            tasks.Add(new Models.Task
            {
                Title = "Team meeting preparation",
                Description = "Prepare slides and agenda for team meeting",
                DueDate = today.AddDays(3),
                ScheduledDate = today.AddDays(3),
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-4)
            });

            tasks.Add(new Models.Task
            {
                Title = "Complete online course module",
                Description = "Finish Module 3 of the Advanced C# course",
                DueDate = today.AddDays(5),
                ScheduledDate = today.AddDays(5),
                CategoryId = johnCategories[4].Id, // Education
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-2)
            });

            tasks.Add(new Models.Task
            {
                Title = "Doctor appointment",
                Description = "Annual checkup with Dr. Johnson",
                DueDate = today.AddDays(2),
                ScheduledDate = today.AddDays(2),
                CategoryId = johnCategories[3].Id, // Health
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-10)
            });

            // ============= THIS MONTH TASKS (Due within 30 days) =============
            tasks.Add(new Models.Task
            {
                Title = "Submit project proposal",
                Description = "Finalize and submit the new project proposal to management",
                DueDate = today.AddDays(15),
                ScheduledDate = today.AddDays(15),
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-20)
            });

            tasks.Add(new Models.Task
            {
                Title = "Update personal blog",
                Description = "Write and publish new blog post about productivity tips",
                DueDate = today.AddDays(12),
                ScheduledDate = today.AddDays(12),
                CategoryId = johnCategories[1].Id, // Personal
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-8)
            });

            tasks.Add(new Models.Task
            {
                Title = "Gym membership renewal",
                Description = "Renew annual gym membership",
                DueDate = today.AddDays(22),
                ScheduledDate = today.AddDays(22),
                CategoryId = johnCategories[3].Id, // Health
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-5)
            });

            // ============= NEXT MONTH TASKS =============
            tasks.Add(new Models.Task
            {
                Title = "Plan vacation",
                Description = "Research and book flights for summer vacation",
                DueDate = today.AddDays(45),
                ScheduledDate = today.AddDays(45),
                CategoryId = johnCategories[1].Id, // Personal
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-3)
            });

            tasks.Add(new Models.Task
            {
                Title = "Performance review meeting",
                Description = "Annual performance review with HR",
                DueDate = today.AddDays(60),
                ScheduledDate = today.AddDays(60),
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-15)
            });

            // ============= OVERDUE TASKS =============
            tasks.Add(new Models.Task
            {
                Title = "Fix billing system bug",
                Description = "Critical: Fix the billing calculation error",
                DueDate = today.AddDays(-5),
                ScheduledDate = today.AddDays(-5),
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-15)
            });

            tasks.Add(new Models.Task
            {
                Title = "Call dentist for appointment",
                Description = "Schedule dental cleaning appointment",
                DueDate = today.AddDays(-10),
                ScheduledDate = today.AddDays(-10),
                CategoryId = johnCategories[3].Id, // Health
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-20)
            });

            tasks.Add(new Models.Task
            {
                Title = "Client feedback review",
                Description = "Review and respond to client feedback from last week",
                DueDate = today.AddDays(-3),
                ScheduledDate = today.AddDays(-3),
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-12)
            });

            // ============= COMPLETED TASKS =============
            tasks.Add(new Models.Task
            {
                Title = "Setup development environment",
                Description = "Install and configure IDE and dependencies",
                DueDate = today.AddDays(-30),
                ScheduledDate = today.AddDays(-30),
                CategoryId = johnCategories[4].Id, // Education
                UserId = users[0].Id,
                IsCompleted = true,
                CompletedAt = today.AddDays(-28),
                CreatedAt = today.AddDays(-45)
            });

            tasks.Add(new Models.Task
            {
                Title = "Database optimization",
                Description = "Optimize slow database queries",
                DueDate = today.AddDays(-15),
                ScheduledDate = today.AddDays(-15),
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = true,
                CompletedAt = today.AddDays(-12),
                CreatedAt = today.AddDays(-25)
            });

            tasks.Add(new Models.Task
            {
                Title = "Buy new clothes",
                Description = "Purchase new winter clothes",
                DueDate = today.AddDays(-20),
                ScheduledDate = today.AddDays(-20),
                CategoryId = johnCategories[2].Id, // Shopping
                UserId = users[0].Id,
                IsCompleted = true,
                CompletedAt = today.AddDays(-18),
                CreatedAt = today.AddDays(-30)
            });

            tasks.Add(new Models.Task
            {
                Title = "Code review",
                Description = "Review pull requests from team members",
                DueDate = today.AddDays(-5),
                ScheduledDate = today.AddDays(-5),
                CategoryId = johnCategories[0].Id, // Work
                UserId = users[0].Id,
                IsCompleted = true,
                CompletedAt = today.AddDays(-2),
                CreatedAt = today.AddDays(-10)
            });

            // ============= JANE'S TASKS =============
            // Today
            tasks.Add(new Models.Task
            {
                Title = "Review deployment logs",
                Description = "Check prod deployment logs for errors",
                DueDate = today,
                ScheduledDate = today,
                CategoryId = janeCategories[0].Id, // Project A
                UserId = users[1].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-2)
            });

            // This week
            tasks.Add(new Models.Task
            {
                Title = "Approve budget allocation",
                Description = "Review and approve Q1 budget allocation",
                DueDate = today.AddDays(4),
                ScheduledDate = today.AddDays(4),
                CategoryId = janeCategories[2].Id, // Admin Tasks
                UserId = users[1].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-6)
            });

            // This month
            tasks.Add(new Models.Task
            {
                Title = "Stakeholder presentation",
                Description = "Present project status to stakeholders",
                DueDate = today.AddDays(18),
                ScheduledDate = today.AddDays(18),
                CategoryId = janeCategories[1].Id, // Project B
                UserId = users[1].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-8)
            });

            // Overdue
            tasks.Add(new Models.Task
            {
                Title = "Vendor contract negotiation",
                Description = "Finalize vendor contract terms",
                DueDate = today.AddDays(-7),
                ScheduledDate = today.AddDays(-7),
                CategoryId = janeCategories[2].Id, // Admin Tasks
                UserId = users[1].Id,
                IsCompleted = false,
                CreatedAt = today.AddDays(-14)
            });

            // Completed
            tasks.Add(new Models.Task
            {
                Title = "Team training session",
                Description = "Conduct training session on new tools",
                DueDate = today.AddDays(-8),
                ScheduledDate = today.AddDays(-8),
                CategoryId = janeCategories[2].Id, // Admin Tasks
                UserId = users[1].Id,
                IsCompleted = true,
                CompletedAt = today.AddDays(-6),
                CreatedAt = today.AddDays(-20)
            });

            return tasks;
        }

        private static List<TaskNote> CreateTaskNotes(List<Models.Task> tasks)
        {
            var notes = new List<TaskNote>();
            var today = DateTime.UtcNow;

            // Add notes to various tasks
            var tasksWithNotes = tasks.Where(t => !t.IsCompleted || t.IsCompleted).Take(12).ToList();

            foreach (var task in tasksWithNotes)
            {
                // Add 2-3 notes to each task
                var noteCount = task.Id % 3 == 0 ? 3 : 2;

                for (int i = 1; i <= noteCount; i++)
                {
                    notes.Add(new TaskNote
                    {
                        Content = GenerateNoteContent(task.Title, i),
                        TaskId = task.Id,
                        CreatedAt = today.AddDays(-(noteCount - i + 1)),
                        UpdatedAt = null
                    });
                }
            }

            return notes;
        }

        private static string GenerateNoteContent(string taskTitle, int noteNumber)
        {
            var noteTemplates = new Dictionary<int, Func<string, string>>
            {
                { 1, title => $"Initial note for {title}: Starting work on this task. Need to review requirements and gather resources." },
                { 2, title => $"Progress update on {title}: Completed initial analysis. Found some issues that need to be addressed." },
                { 3, title => $"Final note for {title}: Task is nearly complete. Ready for testing and validation by team lead." }
            };

            return noteTemplates.ContainsKey(noteNumber) 
                ? noteTemplates[noteNumber](taskTitle) 
                : $"Additional note {noteNumber} for {taskTitle}: Ongoing update and tracking.";
        }

        private static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password);
        }
    }
}
