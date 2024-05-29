using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExerciseRepository> _logger;

    public ExerciseRepository(ApplicationDbContext context, ILogger<ExerciseRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Exercise>> GetAllExercisesForWorkoutAsync(Guid workoutId)
    {
        _logger.LogInformation("Getting all exercises for workout {WorkoutId}", workoutId);
        return await _context.Exercises
                             .Where(e => e.WorkoutId == workoutId)
                             .Include(e => e.Series)
                             .ToListAsync();
    }

    public async Task<Exercise> GetExerciseByIdAsync(Guid exerciseId)
    {
        _logger.LogInformation("Getting exercise by id {ExerciseId}", exerciseId);
        return await _context.Exercises
                             .Include(e => e.Series)
                             .FirstOrDefaultAsync(e => e.Id == exerciseId);
    }

    public async Task<Exercise> CreateExerciseAsync(Exercise exercise)
    {
        if (exercise == null) throw new ArgumentNullException(nameof(exercise));

        _logger.LogInformation("Creating exercise for workout {WorkoutId}", exercise.WorkoutId);

        if (await _context.Workouts.AnyAsync(w => w.Id == exercise.WorkoutId))
        {
            await _context.Exercises.AddAsync(exercise);
            await _context.SaveChangesAsync();
            return exercise;
        }
        else
        {
            _logger.LogWarning("Attempt to create exercise for non-existing workout {WorkoutId}", exercise.WorkoutId);
            throw new InvalidOperationException("Exercise cannot be created without a valid workout.");
        }
    }

    public async Task UpdateExerciseAsync(Exercise exercise)
    {
        if (exercise == null) throw new ArgumentNullException(nameof(exercise));

        var existingExercise = await _context.Exercises.FindAsync(exercise.Id);
        if (existingExercise == null)
        {
            _logger.LogWarning("Attempt to update non-existing exercise {ExerciseId}", exercise.Id);
            throw new InvalidOperationException("Exercise to update not found.");
        }

        _logger.LogInformation("Updating exercise {ExerciseId}", exercise.Id);

        _context.Entry(existingExercise).CurrentValues.SetValues(exercise);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExerciseAsync(Guid exerciseId)
    {
        _logger.LogInformation("Deleting exercise {ExerciseId}", exerciseId);

        var exercise = await _context.Exercises.FindAsync(exerciseId);
        if (exercise != null)
        {
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing exercise {ExerciseId}", exerciseId);
            throw new InvalidOperationException("Exercise to delete not found.");
        }
    }
}