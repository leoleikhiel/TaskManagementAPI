using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public interface ITasksService
    {
        Task<IEnumerable<Models.Task>> GetAllTasksAsync(int userId);
        Task<Models.Task?> GetTaskByIdAsync(int taskId, int userId);
        Task<Models.Task> CreateTaskAsync(CreateTaskDto taskDto, int userId);
        Task<Models.Task?> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto, int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
        Task<int> CompleteAllTasksAsync(int userId);
        Task<IEnumerable<Models.Task>> SearchTasksAsync(string title, int userId);
        Task<IEnumerable<Models.Task>> FilterTasksAsync(bool? isCompleted, int? categoryId,int userId);
        Task<object> GetStatisticsAsync(int userId);
        Task<List<TaskListItemDto>> GetTasksForTodayAsync(int userId);
        Task<List<TaskListItemDto>> GetTasksForWeekAsync(int userId);
        Task<List<TaskListItemDto>> GetOverdueTasksAsync(int userId);
        Task<List<CalendarGroupDto>> GetTasksGroupedByDateAsync(DateTime startDate, DateTime endDate, int userId);
    }
}
