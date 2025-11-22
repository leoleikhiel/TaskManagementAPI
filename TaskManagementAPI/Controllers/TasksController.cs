using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _context.Tasks
                .Include(t => t.Category)
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto taskDto)
        {
            var newTask = new Models.Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                CategoryId = taskDto.CategoryId,
                DueDate = taskDto.DueDate
            };

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id }, newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            task.Title = updateTaskDto.Title ?? task.Title;
            task.Description = updateTaskDto.Description ?? task.Description;
            task.IsCompleted = updateTaskDto.IsCompleted ?? task.IsCompleted;
            task.CategoryId = updateTaskDto.CategoryId ?? task.CategoryId;
            task.DueDate = updateTaskDto.DueDate ?? task.DueDate;

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("complete-all")]
        public async Task<IActionResult> CompleteAllTasks()
        {
            var updatedCount = await _context.Tasks
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsCompleted, true));

            return Ok(new { message = "All tasks completed!", count = updatedCount });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTasks([FromQuery] string title)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Search title cannot be empty");
            }

            var tasks = await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.Title.Contains(title))
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterTasks([FromQuery] bool? isCompleted, [FromQuery] int? categoryId)
        {
            var query = _context.Tasks.Include(t => t.Category).AsQueryable();

            if(isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            if(categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            var tasks = await query.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            var now = DateTime.UtcNow;

            var tasks = await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.IsCompleted == false && t.DueDate.HasValue && t.DueDate < now)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var totalTasks = await _context.Tasks.CountAsync();
            var completedTasks = await _context.Tasks.CountAsync(t => t.IsCompleted);
            var incompleteTasks = totalTasks - completedTasks;

            var tasksByCategory = await _context.Tasks
                .Include(t => t.Category)
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key ?? "Uncategorized",
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(new
            {
                total = totalTasks,
                completedTasks = completedTasks,
                incompleteTasks = incompleteTasks,
                completionRate = totalTasks > 0 ? (completedTasks * 100.0 / totalTasks) : 0,
                byCategory = tasksByCategory
            });
        }
    }
}
