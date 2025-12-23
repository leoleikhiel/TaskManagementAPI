using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface IGoogleCalendarService
    {
        // OAuth Flow
        Task<string> GetAuthorizationUrlAsync(int userId);
        Task<bool> HandleOAuthCallbackAsync(int userId, string authorizationCode);
        Task<bool> DisconnectCalendarAsync(int userId);

        // Token Management
        Task<bool> HasValidTokenAsync(int userId);
        Task<bool> RefreshAccessTokenAsync(int userId);

        // Sync Operations
        Task<TaskSyncResponse> SyncTaskToCalendarAsync(int userId, int taskId);
        Task<TaskSyncResponse> UpdateCalendarEventAsync(int userId, int taskId);
        Task<bool> DeleteCalendarEventAsync(int userId, int taskId);
        Task<List<TaskSyncResponse>> SyncAllTasksAsync(int userId);

        // Status
        Task<CalendarSyncStatusResponse> GetSyncStatusAsync(int userId);
    }
}