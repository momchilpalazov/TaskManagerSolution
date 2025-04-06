

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

        public async Task<IEnumerable<TaskDto>> FilterTasksAsync(FilterTasksDto filter)
        {
            var allTasks = await _unitOfWork.TaskItems.GetAllAsync();

            var filtered = allTasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                Enum.TryParse<Domain.Entities.TaskStatus>(filter.Status, out var status);
                filtered = filtered.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(filter.Priority))
            {
                Enum.TryParse<TaskPriority>(filter.Priority, out var priority);
                filtered = filtered.Where(t => t.Priority == priority);
            }

            if (filter.AssignedToUserId.HasValue)
            {
                filtered = filtered.Where(t => t.AssignedToUserId == filter.AssignedToUserId.Value);
            }

            if (filter.DueBefore.HasValue)
            {
                filtered = filtered.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date <= filter.DueBefore.Value.Date);
            }

            var users = await _unitOfWork.Users.GetAllAsync();
            var userDictionary = users.ToDictionary(u => u.Id, u => u.Email);

            return filtered.Select(task => new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                AssignedToEmail = userDictionary.ContainsKey(task.AssignedToUserId) ? userDictionary[task.AssignedToUserId] : ""
            });
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

        public async Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto dto)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return false;

            Enum.TryParse<Domain.Entities.TaskStatus>(dto.Status, out var status);
            Enum.TryParse<TaskPriority>(dto.Priority, out var priority);

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = status;
            task.Priority = priority;
            task.DueDate = dto.DueDate;

            _unitOfWork.TaskItems.Update(task);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null) return false;

            _unitOfWork.TaskItems.Delete(task);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

    }
}
