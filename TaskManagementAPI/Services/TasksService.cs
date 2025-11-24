using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class TasksService : ITasksService
    {
        private readonly ApplicationDbContext _context;

        public TasksService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Task>> GetAllTasksAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<Models.Task?> GetTaskByIdAsync(int taskId, int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<Models.Task> CreateTaskAsync(CreateTaskDto taskDto, int userId)
        {
            var createCategoryId = taskDto.CategoryId;
            if (taskDto.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == taskDto.CategoryId.Value && c.UserId == userId);
                if (category == null)
                {
                    createCategoryId = null;
                }
            }

            var task = new Models.Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                CategoryId = createCategoryId,
                UserId = userId
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Models.Task?> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto, int userId)
        {
            var task = await GetTaskByIdAsync(taskId, userId);
            if (task == null) return null;
            var updateCategoryId = task.CategoryId;
            if (taskDto.CategoryId.HasValue)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == taskDto.CategoryId.Value && c.UserId == userId);
                if (category == null)
                {
                    updateCategoryId = null;
                }
                else
                {
                    updateCategoryId = taskDto.CategoryId;
                }
            }
            task.Title = taskDto.Title ?? task.Title;
            task.Description = taskDto.Description ?? task.Description;
            task.DueDate = taskDto.DueDate ?? task.DueDate;
            task.CategoryId = updateCategoryId;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await GetTaskByIdAsync(taskId, userId);
            if (task == null) return false;
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CompleteAllTasksAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId && !t.IsCompleted)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsCompleted, true));
        }

        public async Task<IEnumerable<Models.Task>> SearchTasksAsync(string title, int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Title!.Contains(title))
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> FilterTasksAsync(bool? isCompleted, int? categoryId, int userId)
        {
            var query = _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .AsQueryable();
            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }
            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.DueDate.HasValue && t.DueDate < now && t.IsCompleted == false)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<object> GetStatisticsAsync(int userId)
        {
            var totalTasks = await _context.Tasks.CountAsync(t => t.UserId == userId);
            var completedTasks = await _context.Tasks.CountAsync(t => t.UserId == userId && t.IsCompleted);
            var pendingTasks = totalTasks - completedTasks;

            var tasksByCategory = await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Category != null ? t.Category.Name : "Uncategorized")
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return new
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                CompletionRate = totalTasks > 0 ? Math.Round(completedTasks * 100.0 / totalTasks, 2) : 0,
                byCategory = tasksByCategory
            };

        }
    }
}
