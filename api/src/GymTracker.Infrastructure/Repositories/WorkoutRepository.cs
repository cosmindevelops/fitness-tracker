using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WorkoutRepository> _logger;

    public WorkoutRepository(ApplicationDbContext context, ILogger<WorkoutRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Workout>> GetAllWorkoutsAsync(Guid userId)
    {
        _logger.LogInformation("Getting all workouts for user {UserId}", userId);
        return await _context.Workouts
                             .Include(w => w.Exercises)
                             .ThenInclude(e => e.Series)
                             .Where(w => w.UserId == userId)
                             .ToListAsync();
    }

    public async Task<Workout> GetWorkoutByIdAsync(Guid userId, Guid workoutId)
    {
        _logger.LogInformation("Getting workout by id {WorkoutId} for user {UserId}", workoutId, userId);
        return await _context.Workouts
                             .Include(w => w.Exercises)
                             .ThenInclude(e => e.Series)
                             .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
    }

    public async Task<IEnumerable<Workout>> GetWorkoutsByNameAsync(Guid userId, string name)
    {
        _logger.LogInformation("Getting workouts by name {Name} for user {UserId}", name, userId);
        return await _context.Workouts
                             .Where(w => w.UserId == userId && w.Notes.Contains(name))
                             .ToListAsync();
    }

    public async Task<Workout> CreateWorkoutAsync(Workout workout)
    {
        if (workout == null) throw new ArgumentNullException(nameof(workout));

        _logger.LogInformation("Creating workout for user {UserId}", workout.UserId);

        await _context.Workouts.AddAsync(workout);
        await _context.SaveChangesAsync();
        return workout;
    }

    public async Task UpdateWorkoutAsync(Workout workout)
    {
        if (workout == null) throw new ArgumentNullException(nameof(workout));

        _logger.LogInformation("Updating workout {WorkoutId} for user {UserId}", workout.Id, workout.UserId);

        _context.Workouts.Update(workout);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkoutAsync(Guid workoutId)
    {
        _logger.LogInformation("Deleting workout {WorkoutId}", workoutId);

        var workout = await _context.Workouts.FindAsync(workoutId);
        if (workout != null)
        {
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing workout {WorkoutId}", workoutId);
            throw new InvalidOperationException("Workout to delete not found.");
        }
    }
}