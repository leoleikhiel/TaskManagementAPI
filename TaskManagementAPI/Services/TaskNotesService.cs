using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Services
{
    public class TaskNotesService : ITaskNotesService
    {
        private readonly ApplicationDbContext _context;

        public TaskNotesService(ApplicationDbContext context)
        {
            _context = context;
        }

        private TaskNoteResponseDto MapToDto(TaskNote taskNote)
        {
            return new TaskNoteResponseDto
            {
                Id = taskNote.Id,
                Content = taskNote.Content,
                CreatedAt = taskNote.CreatedAt,
                UpdatedAt = taskNote.UpdatedAt,
                TaskId = taskNote.TaskId,
            };
        } 

        public async Task<bool> IsTaskFromUserAsync(int taskId, int userId)
        {
            return await _context.Tasks.AnyAsync(t => t.Id == taskId && t.UserId == userId);
        }

        public async Task<bool> IsTaskNoteExistsAsync(int taskNoteId)
        {
            return await _context.TaskNotes.AnyAsync(tn => tn.Id == taskNoteId);
        }

        public async Task<bool> IsTaskNoteEditableAsync(int taskNoteId)
        {
            var taskNote = await _context.TaskNotes.FirstOrDefaultAsync(tn => tn.Id == taskNoteId);
            if (taskNote == null) return false;
            var hoursSinceCreation = (DateTime.UtcNow - taskNote.CreatedAt).TotalHours;
            return (hoursSinceCreation <= 1);
        }

        public async Task<List<TaskNoteResponseDto>> GetAllTaskNotesByTaskIdAsync(int taskId)
        {
            var taskNotes = await _context.TaskNotes
                .Where(tn => tn.TaskId == taskId)
                .OrderByDescending(tn => tn.CreatedAt)
                .ToListAsync();

            return taskNotes.Select(MapToDto).ToList();
        }

        public async Task<TaskNoteResponseDto?> GetTaskNoteByIdAsync(int taskNoteId)
        {
            var taskNote = await _context.TaskNotes
                .FirstOrDefaultAsync(tn => tn.Id == taskNoteId);

            if(taskNote == null)
            {
                return null;
            }

            return MapToDto(taskNote);
        }

        public async Task<TaskNoteResponseDto> CreateTaskNoteAsync(int taskId, TaskNoteDto taskNoteDto)
        {
            var taskNote = new TaskNote
            {
                Content = taskNoteDto.Content,
                CreatedAt = DateTime.UtcNow,
                TaskId = taskId
            };

            _context.TaskNotes.Add(taskNote);
            await _context.SaveChangesAsync();

            return MapToDto(taskNote);
        }

        public async Task<TaskNoteResponseDto?> UpdateTaskNoteAsync(int taskNoteId, TaskNoteDto taskNoteDto)
        {
            var taskNote = await _context.TaskNotes.FirstOrDefaultAsync(x => x.Id == taskNoteId);

            if (taskNote == null) return null;

            taskNote.Content = taskNoteDto.Content;
            taskNote.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(taskNote);
        }

        public async Task<bool> DeleteTaskNoteAsync(int taskNoteId)
        {
            var taskNote = await _context.TaskNotes.FirstOrDefaultAsync(tn => tn.Id == taskNoteId);
            if (taskNote == null) return false;
            _context.TaskNotes.Remove(taskNote);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
