using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserExistsAsync(string username, string email)
    {
        return await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
    }

    public async Task<User> RegisterUserAsync(User user, string roleName)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role != null)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        return user;
    }

    public async Task<User> FindByUsernameAsync(string username)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<string>> GetUserRolesAsync(User user)
    {
        return await _context.UserRoles.Where(ur => ur.UserId == user.Id)
                                        .Include(ur => ur.Role)
                                        .Select(ur => ur.Role.Name)
                                        .ToListAsync();
    }

    public async Task<User> FindByIdAsync(Guid userId)
    {
        return await _context.Users
                             .Include(u => u.UserRoles)
                             .ThenInclude(ur => ur.Role)
                             .SingleOrDefaultAsync(u => u.Id == userId);
    }
}