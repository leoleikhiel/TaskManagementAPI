using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class GoogleCalendarToken
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string RefreshToken { get; set; } = string.Empty;

        [Required]
        public DateTime TokenExpiry { get; set; }

        [MaxLength(255)]
        public string? CalendarId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User? User { get; set; }

        public bool IsExpired => DateTime.UtcNow >= TokenExpiry;
    }
}
