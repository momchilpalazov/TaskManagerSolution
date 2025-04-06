

using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);

    }
}
