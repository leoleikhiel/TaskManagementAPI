using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface ITaskNotesService
    {
        Task<bool> IsTaskFromUserAsync(int taskId, int userId);
        Task<bool> IsTaskNoteExistsAsync(int taskNoteId);
        Task<bool> IsTaskNoteEditableAsync(int taskNoteId);
        Task<List<TaskNoteResponseDto>> GetAllTaskNotesByTaskIdAsync(int taskId);
        Task<TaskNoteResponseDto?> GetTaskNoteByIdAsync(int taskNoteId);
        Task<TaskNoteResponseDto> CreateTaskNoteAsync(int taskId, TaskNoteDto taskNoteDto);
        Task<TaskNoteResponseDto?> UpdateTaskNoteAsync(int taskNoteId, TaskNoteDto taskNoteDto);
        Task<bool> DeleteTaskNoteAsync(int taskNoteId);
    }
}