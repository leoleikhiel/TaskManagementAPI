using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs
{
    public class CalendarAuthUrlResponse
    {
        public string AuthUrl { get; set; } = string.Empty;
    }

    public class CalendarSyncStatusResponse
    {
        public bool IsConnected { get; set; }
        public DateTime? LastSync { get; set; }
        public int SyncedTasksCount { get; set; }
        public int FailedTasksCount { get; set; }
        public int PendingTasksCount { get; set; }
    }

    public class TaskSyncResponse
    {
        public int TaskId { get; set; }
        public bool Success { get; set; }
        public string? GoogleEventId { get; set; }
        public DateTime? LastSynced { get; set; }
        public string? Message { get; set; }
    }

    public class CalendarAuthRequest
    {
        [Required]
        public string AuthorizationCode { get; set; } = string.Empty;
    }

    public class CalendarSyncRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TaskId must be greater than 0")]
        public int TaskId { get; set; }
    }
}
