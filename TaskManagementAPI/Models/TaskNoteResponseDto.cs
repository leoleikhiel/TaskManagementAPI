using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskNoteResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TaskId { get; set; }
    }
}
