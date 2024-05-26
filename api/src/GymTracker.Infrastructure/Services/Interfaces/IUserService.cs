using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> FindByIdAsync(Guid userId);
}