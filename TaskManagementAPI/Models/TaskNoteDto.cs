using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskNoteDto
    {
        [Required(ErrorMessage = "Content is required!")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 2000 characters")]
        public string Content { get; set; } = string.Empty;
    }
}
