

using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services
{
    public class DashboardService : IDashboardService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetSummaryAsync()
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            var completedTasks = tasks.Where(t => t.Status == Domain.Entities.TaskStatus.Done).Count();
            var uniqueUserIds = tasks.Select(t => t.AssignedToUserId).Distinct().Count();

            return new DashboardDto
            {
                TotalProjects = projects.Count,
                TotalTasks = tasks.Count,
                CompletedTasks = completedTasks,
                ActiveUsers = uniqueUserIds
            };
        }
    }
    
}
