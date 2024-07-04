using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class UserExerciseProgressRepository : IUserExerciseProgressRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserExerciseProgressRepository> _logger;

    public UserExerciseProgressRepository(ApplicationDbContext context, ILogger<UserExerciseProgressRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserExerciseProgress>> GetUserExerciseProgressByUserWorkoutTemplateIdAsync(Guid userWorkoutTemplateId)
    {
        _logger.LogInformation("Getting user exercise progress for user workout template {TemplateId}", userWorkoutTemplateId);
        return await _context.UserExerciseProgresses
                             .Where(uep => uep.UserWorkoutTemplateId == userWorkoutTemplateId)
                             .Include(uep => uep.TemplateExercise)
                             .ToListAsync();
    }

    public async Task<UserExerciseProgress> GetUserExerciseProgressByIdAsync(Guid progressId)
    {
        _logger.LogInformation("Getting user exercise progress by id {ProgressId}", progressId);
        return await _context.UserExerciseProgresses
                             .Include(uep => uep.TemplateExercise)
                             .FirstOrDefaultAsync(uep => uep.Id == progressId);
    }

    public async Task<UserExerciseProgress> CreateUserExerciseProgressAsync(UserExerciseProgress userExerciseProgress)
    {
        if (userExerciseProgress == null) throw new ArgumentNullException(nameof(userExerciseProgress));

        _logger.LogInformation("Creating user exercise progress for user workout template {TemplateId}", userExerciseProgress.UserWorkoutTemplateId);

        await _context.UserExerciseProgresses.AddAsync(userExerciseProgress);
        await _context.SaveChangesAsync();
        return userExerciseProgress;
    }

    public async Task UpdateUserExerciseProgressAsync(UserExerciseProgress userExerciseProgress)
    {
        if (userExerciseProgress == null) throw new ArgumentNullException(nameof(userExerciseProgress));

        _logger.LogInformation("Updating user exercise progress {ProgressId}", userExerciseProgress.Id);

        _context.UserExerciseProgresses.Update(userExerciseProgress);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserExerciseProgressAsync(Guid progressId)
    {
        _logger.LogInformation("Deleting user exercise progress {ProgressId}", progressId);

        var userExerciseProgress = await _context.UserExerciseProgresses.FindAsync(progressId);
        if (userExerciseProgress != null)
        {
            _context.UserExerciseProgresses.Remove(userExerciseProgress);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing user exercise progress {ProgressId}", progressId);
            throw new InvalidOperationException("User exercise progress to delete not found.");
        }
    }
}