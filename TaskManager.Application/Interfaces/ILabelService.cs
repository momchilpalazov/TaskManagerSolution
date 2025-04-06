

using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface ILabelService
    {
        Task<IEnumerable<LabelDto>> GetAllAsync();
        Task<LabelDto> CreateAsync(string name);
    }
}
