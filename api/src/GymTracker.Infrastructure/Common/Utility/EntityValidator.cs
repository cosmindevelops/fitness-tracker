using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Common.Utility.Interfaces;
using GymTracker.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Common.Utility;

public class EntityValidator : IEntityValidator
{
    private readonly ILogger<EntityValidator> _logger;

    public EntityValidator(ILogger<EntityValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void EnsureWorkoutExists(Workout workout, Guid workoutId)
    {
        if (workout == null)
        {
            _logger.LogError("Workout with ID {WorkoutId} not found", workoutId);
            throw new WorkoutNotFoundException($"Workout with ID {workoutId} not found for this user.");
        }
    }

    public void EnsureExerciseExists(Exercise exercise, Guid exerciseId, Guid workoutId)
    {
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            _logger.LogError("Exercise with ID {ExerciseId} not found for workout {WorkoutId}", exerciseId, workoutId);
            throw new ExerciseNotFoundException($"Exercise with ID {exerciseId} not found for this workout.");
        }
    }

    public void EnsureSeriesExists(Series series, Guid seriesId, Guid exerciseId)
    {
        if (series == null || series.ExerciseId != exerciseId)
        {
            _logger.LogError("Series with ID {SeriesId} not found for exercise {ExerciseId}", seriesId, exerciseId);
            throw new SeriesNotFoundException($"Series with ID {seriesId} not found for this exercise.");
        }
    }

    public void EnsureUserExists(User user, Guid userId)
    {
        if (user == null)
        {
            _logger.LogError("User with ID {UserId} not found", userId);
            throw new UserNotFoundException($"User with ID {userId} not found.");
        }
    }
}