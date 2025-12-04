using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.Services;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Authorize]
    [Route("api/tasks/{taskId}/notes")]
    [ApiController]
    public class TaskNotesController : ControllerBase
    {
        private readonly ITaskNotesService _taskNotesService;

        public TaskNotesController(ITaskNotesService taskNotesService)
        {
            _taskNotesService = taskNotesService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }

        private async Task<ActionResult?> ValidateTaskOwnership(int taskId)
        {
            var userId = GetCurrentUserId();
            if(!await _taskNotesService.IsTaskFromUserAsync(taskId, userId))
            {
                return Forbid();
            }
            return null;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskNoteResponseDto>>> GetAllTaskNotesByTaskId(int taskId)
        {
            var validationResult = await ValidateTaskOwnership(taskId);
            if (validationResult != null) return validationResult;

            var taskNotes = await _taskNotesService.GetAllTaskNotesByTaskIdAsync(taskId);

            return Ok(taskNotes);
        }

        [HttpGet("{taskNoteId}")]
        public async Task<ActionResult<TaskNoteResponseDto>> GetTaskNoteById(int taskId, int taskNoteId)
        {
            var validationResult = await ValidateTaskOwnership(taskId);
            if (validationResult != null) return validationResult;

            if (!await _taskNotesService.IsTaskNoteExistsAsync(taskNoteId))
            {
                return NotFound();
            }

            var taskNote = await _taskNotesService.GetTaskNoteByIdAsync(taskNoteId);
            return Ok(taskNote);
        }

        [HttpPost]
        public async Task<ActionResult<TaskNoteResponseDto>> CreateTaskNote(int taskId, TaskNoteDto taskNoteDto)
        {
            var validationResult = await ValidateTaskOwnership(taskId);
            if (validationResult != null) return validationResult;

            var taskNote = await _taskNotesService.CreateTaskNoteAsync(taskId, taskNoteDto);

            return CreatedAtAction(nameof(GetTaskNoteById), new { taskId = taskId, taskNoteId = taskNote.Id }, taskNote);
        }

        [HttpPut("{taskNoteId}")]
        public async Task<ActionResult<TaskNoteResponseDto>> UpdateTaskNote(int taskId, int taskNoteId, TaskNoteDto taskNoteDto)
        {
            var validationResult = await ValidateTaskOwnership(taskId);
            if (validationResult != null) return validationResult;

            if (!await _taskNotesService.IsTaskNoteExistsAsync(taskNoteId))
            {
                return NotFound();
            }

            var tempTaskNote = await _taskNotesService.GetTaskNoteByIdAsync(taskNoteId);

            if(tempTaskNote.TaskId != taskId)
            {
                return BadRequest("Note does not belong to this task");
            }

            if(!await _taskNotesService.IsTaskNoteEditableAsync(taskNoteId))
            {
                return BadRequest(new
                {
                    error = "Edit window expired",
                    message = "Notes can only be edited within 1 hour of creation"
                });
            }

            var taskNote = await _taskNotesService.UpdateTaskNoteAsync(taskNoteId, taskNoteDto);

            return Ok(taskNote);
        }

        [HttpDelete("{taskNoteId}")]
        public async Task<IActionResult> DeleteTaskNote(int taskId, int taskNoteId)
        {
            var validationResult = await ValidateTaskOwnership(taskId);
            if (validationResult != null) return validationResult;

            if (!await _taskNotesService.IsTaskNoteExistsAsync(taskNoteId))
            {
                return NotFound();
            }

            await _taskNotesService.DeleteTaskNoteAsync(taskNoteId);

            return NoContent();
        }
    }
}
