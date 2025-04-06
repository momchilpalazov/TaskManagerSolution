

using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
        Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(Guid projectId);
    }
}
