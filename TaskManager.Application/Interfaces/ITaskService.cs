

using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
        Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(Guid projectId);
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<IEnumerable<TaskDto>> FilterTasksAsync(FilterTasksDto filter);
        Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto dto);
        Task<bool> DeleteTaskAsync(Guid id);
        Task<IEnumerable<TaskDto>> GetTasksForUserAsync(Guid userId);
        Task<IEnumerable<TaskDto>> GetOverdueTasksAsync(Guid userId);
        Task<IEnumerable<TaskDto>> SearchTasksAsync(Guid userId, string keyword);
        Task<IEnumerable<TaskDto>> GetTasksForTodayAsync(Guid userId);
        Task<IEnumerable<TaskDto>> GetTasksForTomorrowAsync(Guid userId);
        Task<IEnumerable<TaskDto>> GetTasksForWeekAsync(Guid userId);





    }
}
