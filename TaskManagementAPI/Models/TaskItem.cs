using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<TaskNote> Notes { get; set; } = new List<TaskNote>();
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsSyncedToCalendar { get; set; } = false;

        [MaxLength(1024)]
        public string GoogleEventId { get; set; } = string.Empty;

        public DateTime? LastCalendarSync { get; set; }
        public TaskCalendarSync? CalendarSync { get; set; }

        [NotMapped]
        public bool IsOverdue => !IsCompleted && DueDate.HasValue && DateTime.UtcNow.Date > DueDate.Value.Date;
    }
}
