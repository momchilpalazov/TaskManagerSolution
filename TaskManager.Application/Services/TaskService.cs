

using AutoMapper;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IEmailService emailService, IAuditService auditService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _auditService = auditService;
            _mapper = mapper;
        }

        // This method is not implemented yet. It should create a new task in the database.
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
        {
            //  Exchange the string values for the enum values
            Enum.TryParse<Domain.Entities.TaskStatus>(dto.Status, out var status);
            Enum.TryParse<TaskPriority>(dto.Priority, out var priority);

            var task = _mapper.Map<TaskItem>(dto);

            if (dto.LabelIds.Any())
            {
                var labels = await _unitOfWork.Labels.FindAsync(l => dto.LabelIds.Contains(l.Id));
                task.TaskLabels = labels.Select(l => new TaskLabel { Label = l }).ToList();
            }


            await _unitOfWork.TaskItems.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(task.AssignedToUserId);

            if (user != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Task Created] '{task.Title}' assigned to user {user.Email}, due by {task.DueDate?.ToShortDateString() ?? "unspecified"}.");
                Console.ResetColor();

                await _emailService.SendAsync(user.Email, "Нова задача ти е възложена",
                $"Заглавие: {task.Title}\nОписание: {task.Description ?? "няма"}\nКраен срок: {task.DueDate?.ToShortDateString() ?? "няма"}");

                await _auditService.LogAsync("Task", "Create", user.Email, $"Задача: {task.Title}");

            }

            return _mapper.Map<TaskDto>(task);


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

        public async Task<IEnumerable<TaskDto>> GetTasksForUserAsync(Guid userId)
        {
            var tasks = await _unitOfWork.TaskItems.FindAsync(t => t.AssignedToUserId == userId);

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

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                AssignedToEmail = users.FirstOrDefault(u => u.Id == t.AssignedToUserId)?.Email ?? ""
            });
        }



        public async Task<IEnumerable<TaskDto>> GetOverdueTasksAsync(Guid userId)
        {
            var now = DateTime.UtcNow.Date;
            var tasks = await _unitOfWork.TaskItems.FindAsync(t =>
                t.AssignedToUserId == userId &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date < now &&
                t.Status != Domain.Entities.TaskStatus.Done
            );

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

        public async Task<IEnumerable<TaskDto>> SearchTasksAsync(Guid userId, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<TaskDto>();

            var tasks = await _unitOfWork.TaskItems.FindAsync(t =>
                t.AssignedToUserId == userId &&
                (
                    t.Title.ToLower().Contains(keyword.ToLower()) ||
                    (t.Description != null && t.Description.ToLower().Contains(keyword.ToLower()))
                )
            );

            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                AssignedToEmail = t.AssignedToUser?.Email ?? ""
            });
        }

        public async Task<IEnumerable<TaskDto>> GetTasksForTodayAsync(Guid userId)
        {
            var today = DateTime.UtcNow.Date;

            var tasks = await _unitOfWork.TaskItems.FindAsync(t =>
                t.AssignedToUserId == userId &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date == today &&
                t.Status != Domain.Entities.TaskStatus.Done
            );

            return tasks.Select(_mapper.Map<TaskDto>);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksForTomorrowAsync(Guid userId)
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);

            var tasks = await _unitOfWork.TaskItems.FindAsync(t =>
                t.AssignedToUserId == userId &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date == tomorrow &&
                t.Status != Domain.Entities.TaskStatus.Done
            );

            return tasks.Select(_mapper.Map<TaskDto>);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksForWeekAsync(Guid userId)
        {
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Понеделник
            var endOfWeek = startOfWeek.AddDays(6); // Неделя

            var tasks = await _unitOfWork.TaskItems.FindAsync(t =>
                t.AssignedToUserId == userId &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date >= startOfWeek &&
                t.DueDate.Value.Date <= endOfWeek &&
                t.Status != Domain.Entities.TaskStatus.Done
            );

            return tasks.Select(_mapper.Map<TaskDto>);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByLabelAsync(Guid userId, Guid labelId)
        {
            var tasks = await _unitOfWork.TaskItems.FindAsync(t =>
                t.AssignedToUserId == userId &&
                t.TaskLabels.Any(tl => tl.LabelId == labelId)
            );

            return tasks.Select(_mapper.Map<TaskDto>);
        }



    }
}
