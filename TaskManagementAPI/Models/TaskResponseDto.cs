namespace TaskManagementAPI.Models
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsOverdue { get; set; }
        public int? CategoryId {  get; set; }
        public string? CategoryName { get; set; }
        public int NotesCount { get; set; }
    }
}
