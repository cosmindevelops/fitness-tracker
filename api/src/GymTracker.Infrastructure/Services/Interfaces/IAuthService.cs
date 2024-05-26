using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string UserId, string Token)> RegisterAsync(RegisterModelDto model);

    Task<(bool Success, string UserId, string Token)> LoginAsync(LoginModelDto model);
}