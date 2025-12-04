using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class UpdateTaskDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }

        public int? CategoryId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}