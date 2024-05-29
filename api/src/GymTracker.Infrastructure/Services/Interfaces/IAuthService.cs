using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterModelDto model);

    Task<AuthResponseDto> LoginAsync(LoginModelDto model);
}