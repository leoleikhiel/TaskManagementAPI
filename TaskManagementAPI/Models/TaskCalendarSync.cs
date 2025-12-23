using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public enum SyncStatus
    {
        Synced = 0,
        Pending = 1,
        Failed = 2
    }

    public class TaskCalendarSync
    {
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(1024)]
        public string GoogleEventId { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public SyncStatus SyncStatus { get; set; } = SyncStatus.Synced;

        public string? ErrorMessage { get; set; }

        public TaskItem? Task { get; set; }
        public User? User { get; set; }
    }
}
