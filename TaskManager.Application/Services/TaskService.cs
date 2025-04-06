

using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {

        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // This method is not implemented yet. It should create a new task in the database.
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
        {
            //  Exchange the string values for the enum values
            Enum.TryParse<Domain.Entities.TaskStatus>(dto.Status, out var status);
            Enum.TryParse<TaskPriority>(dto.Priority, out var priority);

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                ProjectId = dto.ProjectId,
                AssignedToUserId = dto.AssignedToUserId,
                Status = status,
                Priority = priority,
                DueDate = dto.DueDate
            };

            await _unitOfWork.TaskItems.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(task.AssignedToUserId);

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                AssignedToEmail = user?.Email ?? ""
            };
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(Guid projectId)
        {
            var tasks = await _unitOfWork.TaskItems.FindAsync(t => t.ProjectId == projectId);

            return tasks.Select(task => new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                AssignedToEmail = task.AssignedToUser?.Email ?? ""
            });
        }
    }
}
