using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Models
{
    public class TaskNote
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Content is required!")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 2000 characters")]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public Models.Task? Task { get; set; }
    }
}
