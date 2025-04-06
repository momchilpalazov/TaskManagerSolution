
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto);
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    }
}
