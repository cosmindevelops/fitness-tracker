using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(string username, string email);
    Task<User> RegisterUserAsync(User user, string roleName);
    Task<User> FindByUsernameAsync(string username);
    Task<List<string>> GetUserRolesAsync(User user);
}