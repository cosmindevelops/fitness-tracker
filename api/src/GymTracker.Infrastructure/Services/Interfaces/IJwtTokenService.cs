using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user, List<string> roles);
}