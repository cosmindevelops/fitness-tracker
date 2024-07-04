using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class UserWorkoutTemplateRepository : IUserWorkoutTemplateRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserWorkoutTemplateRepository> _logger;

    public UserWorkoutTemplateRepository(ApplicationDbContext context, ILogger<UserWorkoutTemplateRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserWorkoutTemplate>> GetUserWorkoutTemplatesByUserIdAsync(Guid userId)
    {
        _logger.LogInformation("Getting user workout templates for user {UserId}", userId);
        return await _context.UserWorkoutTemplates
                             .Where(uwt => uwt.UserId == userId)
                             .Include(uwt => uwt.WorkoutTemplate)
                             .ToListAsync();
    }

    public async Task<UserWorkoutTemplate> GetUserWorkoutTemplateByIdAsync(Guid templateId)
    {
        _logger.LogInformation("Getting user workout template by id {TemplateId}", templateId);
        return await _context.UserWorkoutTemplates
                             .Include(uwt => uwt.WorkoutTemplate)
                             .Include(uwt => uwt.UserExerciseProgress)
                             .FirstOrDefaultAsync(uwt => uwt.Id == templateId);
    }

    public async Task<UserWorkoutTemplate> CreateUserWorkoutTemplateAsync(UserWorkoutTemplate userWorkoutTemplate)
    {
        if (userWorkoutTemplate == null) throw new ArgumentNullException(nameof(userWorkoutTemplate));

        _logger.LogInformation("Creating user workout template for user {UserId}", userWorkoutTemplate.UserId);

        await _context.UserWorkoutTemplates.AddAsync(userWorkoutTemplate);
        await _context.SaveChangesAsync();
        return userWorkoutTemplate;
    }

    public async Task UpdateUserWorkoutTemplateAsync(UserWorkoutTemplate userWorkoutTemplate)
    {
        if (userWorkoutTemplate == null) throw new ArgumentNullException(nameof(userWorkoutTemplate));

        _logger.LogInformation("Updating user workout template {TemplateId}", userWorkoutTemplate.Id);

        _context.UserWorkoutTemplates.Update(userWorkoutTemplate);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserWorkoutTemplateAsync(Guid templateId)
    {
        _logger.LogInformation("Deleting user workout template {TemplateId}", templateId);

        var userWorkoutTemplate = await _context.UserWorkoutTemplates.FindAsync(templateId);
        if (userWorkoutTemplate != null)
        {
            _context.UserWorkoutTemplates.Remove(userWorkoutTemplate);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing user workout template {TemplateId}", templateId);
            throw new InvalidOperationException("User workout template to delete not found.");
        }
    }

    public async Task<UserWorkoutTemplate> GetUserWorkoutTemplateByUserIdAndTemplateIdAsync(Guid userId, Guid templateId)
    {
        return await _context.UserWorkoutTemplates
            .FirstOrDefaultAsync(uwt => uwt.UserId == userId && uwt.WorkoutTemplateId == templateId);
    }
}