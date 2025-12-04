namespace TaskManagementAPI.Models
{
    public class CalendarGroupDto
    {
        public DateTime Date { get; set; }
        public List<TaskListItemDto> Tasks { get; set; } = new List<TaskListItemDto>();
    }
}
