using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;

        public TasksController(ITasksService tasksService)
        {
            _tasksService = tasksService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var userId = GetCurrentUserId();

            var tasks = await _tasksService.GetAllTasksAsync(userId);

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var userId = GetCurrentUserId();

            var task = await _tasksService.GetTaskByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto taskDto)
        {
            var userId = GetCurrentUserId();

            var newTask = await _tasksService.CreateTaskAsync(taskDto, userId);

            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id }, newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            var userId = GetCurrentUserId();

            var task = await _tasksService.UpdateTaskAsync(id, updateTaskDto, userId);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();

            var deleted = await _tasksService.DeleteTaskAsync(id, userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("complete-all")]
        public async Task<IActionResult> CompleteAllTasks()
        {
            var userId = GetCurrentUserId();
            var count = await _tasksService.CompleteAllTasksAsync(userId);

            return Ok(new { message = "All tasks completed!", count });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTasks([FromQuery] string title)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Search title cannot be empty");
            }

            var userId = GetCurrentUserId();
            var tasks = await _tasksService.SearchTasksAsync(title, userId);

            return Ok(tasks);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterTasks([FromQuery] bool? isCompleted, [FromQuery] int? categoryId)
        {
            var userId = GetCurrentUserId();
            var tasks = await _tasksService.FilterTasksAsync(isCompleted, categoryId, userId);

            return Ok(tasks);
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            var userId = GetCurrentUserId();
            var now = DateTime.UtcNow;
            var tasks = await _tasksService.GetOverdueTasksAsync(userId);

            return Ok(tasks);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var userId = GetCurrentUserId();
            var stats = await _tasksService.GetStatisticsAsync(userId);

            return Ok(stats);
        }
    }
}
