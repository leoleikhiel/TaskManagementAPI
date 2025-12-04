namespace TaskManagementAPI.Models
{
    public class TaskListItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public bool IsOverdue { get; set; }
        public string? CategoryName { get; set; }
        public int NotesCount { get; set; }
    }
}
