
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(RegisterUserDto dto);
    }
}
