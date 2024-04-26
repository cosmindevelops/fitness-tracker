using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Exceptions;

namespace GymTracker.Infrastructure.Common.Utility;

public class EntityValidator
{
    public static void EnsureWorkoutExists(Workout workout, Guid workoutId)
    {
        if (workout == null)
        {
            throw new WorkoutNotFoundException($"Workout with ID {workoutId} not found for this user.");
        }
    }

    public static void EnsureExerciseExists(Exercise exercise, Guid exerciseId, Guid workoutId)
    {
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            throw new ExerciseNotFoundException($"Exercise with ID {exerciseId} not found for this workout.");
        }
    }

    public static void EnsureSeriesExists(Series series, Guid seriesId, Guid exerciseId)
    {
        if (series == null || series.ExerciseId != exerciseId)
        {
            throw new SeriesNotFoundException($"Series with ID {seriesId} not found for this exercise.");
        }
    }

    public static void EnsureUserExists(User user, Guid userId)
    {
        if (user == null)
        {
            throw new UserNotFoundException($"User with ID {userId} not found.");
        }
    }

    public static void EnsureSeriesListExists(Series series, Guid seriesId, Guid exerciseId)
    {
        if (series == null || series.ExerciseId != exerciseId)
        {
            throw new SeriesNotFoundException($"Series with ID {seriesId} not found for this exercise.");
        }
    }
}