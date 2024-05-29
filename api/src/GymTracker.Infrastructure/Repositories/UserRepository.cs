using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> UserExistsAsync(string username, string email)
    {
        _logger.LogInformation("Checking if user exists with username {Username} or email {Email}", username, email);
        return await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
    }

    public async Task<User> RegisterUserAsync(User user, string roleName)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        _logger.LogInformation("Registering user {Username} with role {RoleName}", user.Username, roleName);

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
        _logger.LogInformation("Finding user by username {Username}", username);
        return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        _logger.LogInformation("Finding user by email {Email}", email);
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<string>> GetUserRolesAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        _logger.LogInformation("Getting roles for user {UserId}", user.Id);

        return await _context.UserRoles.Where(ur => ur.UserId == user.Id)
                                        .Include(ur => ur.Role)
                                        .Select(ur => ur.Role.Name)
                                        .ToListAsync();
    }

    public async Task<User> FindByIdAsync(Guid userId)
    {
        _logger.LogInformation("Finding user by id {UserId}", userId);
        return await _context.Users
                             .Include(u => u.UserRoles)
                             .ThenInclude(ur => ur.Role)
                             .SingleOrDefaultAsync(u => u.Id == userId);
    }
}