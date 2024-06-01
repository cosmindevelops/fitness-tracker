using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using Microsoft.AspNetCore.Http;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterModelDto model);

    Task<AuthResponseDto> LoginAsync(LoginModelDto model);

    Task<User> FindOrCreateGoogleUserAsync(string email);

    Task<(User, string)> HandleGoogleResponse(HttpContext httpContext);
}