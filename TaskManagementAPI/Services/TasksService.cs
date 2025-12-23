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

        private DateTime? GetDisplayDate(TaskItem task)
        {
            return task.ScheduledDate ?? task.DueDate;
        }

        private TaskListItemDto MapToListItemDto(TaskItem task)
        {
            return new TaskListItemDto
            {
                Id = task.Id,
                Title = task.Title,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate,
                ScheduledDate = task.ScheduledDate,
                IsOverdue = task.IsOverdue,
                CategoryName = task.Category != null ? task.Category.Name : null,
                NotesCount = task.Notes.Count
            };
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId, int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<TaskItem> CreateTaskAsync(CreateTaskDto taskDto, int userId)
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

            var task = new TaskItem
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

        public async Task<TaskItem?> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto, int userId)
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
            task.ScheduledDate = taskDto.ScheduledDate ?? task.ScheduledDate;
            task.CategoryId = updateCategoryId;

            if(taskDto.IsCompleted.HasValue)
            {
                if(taskDto.IsCompleted.Value && !task.IsCompleted)
                {
                    task.IsCompleted = true;
                    task.CompletedAt = taskDto.CompletedAt ?? DateTime.UtcNow;
                }
                else if(!taskDto.IsCompleted.Value && task.IsCompleted)
                {
                    task.IsCompleted = false;
                    task.CompletedAt = null;
                }
            }

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

        public async Task<IEnumerable<TaskItem>> SearchTasksAsync(string title, int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Title!.Contains(title))
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> FilterTasksAsync(bool? isCompleted, int? categoryId, int userId)
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

        public async Task<List<TaskListItemDto>> GetTasksForTodayAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .Where(t => (t.DueDate.HasValue && t.DueDate.Value.Date == today)
                            || (t.ScheduledDate.HasValue && t.ScheduledDate.Value.Date == today))
                .Include(t => t.Category)
                .Include(t => t.Notes)
                .ToListAsync();

            return tasks.Select(MapToListItemDto).ToList();
        }

        public async Task<List<TaskListItemDto>> GetTasksForWeekAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var startOfWeek = today.AddDays(-daysSinceMonday);

            var endOfWeek = startOfWeek.AddDays(6);

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .Where(t => (t.ScheduledDate.HasValue && t.ScheduledDate.Value.Date >= startOfWeek && t.ScheduledDate.Value.Date <= endOfWeek)
                            || (!t.ScheduledDate.HasValue && t.DueDate.HasValue && t.DueDate.Value.Date >= startOfWeek && t.DueDate.Value.Date <= endOfWeek))
                .Include(t => t.Category)
                .Include(t => t.Notes)
                .ToListAsync();

            return tasks.Select(MapToListItemDto).ToList();
        }

        public async Task<List<TaskListItemDto>> GetOverdueTasksAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value.Date < today)
                .Include(t => t.Category)
                .Include(t => t.Notes)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return tasks.Select(MapToListItemDto).ToList();
        }

        public async Task<List<CalendarGroupDto>> GetTasksGroupedByDateAsync(DateTime startDate, DateTime endDate, int userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .Where(t => (t.ScheduledDate.HasValue && t.ScheduledDate.Value.Date >= startDate.Date && t.ScheduledDate.Value.Date <= endDate.Date)
                            || (!t.ScheduledDate.HasValue && t.DueDate.HasValue && t.DueDate.Value.Date >= startDate.Date && t.DueDate.Value.Date <= endDate.Date))
                .Include(t => t.Category)
                .Include(t => t.Notes)
                .ToListAsync();

            var grouped = tasks
                .GroupBy(t => GetDisplayDate(t)?.Date)
                .Where(g => g.Key.HasValue)
                .OrderBy(g => g.Key)
                .Select(g => new CalendarGroupDto
                {
                    Date = g.Key.Value,
                    Tasks = g.Select(MapToListItemDto).ToList()
                })
                .ToList();

            return grouped;
        }
    }
}
