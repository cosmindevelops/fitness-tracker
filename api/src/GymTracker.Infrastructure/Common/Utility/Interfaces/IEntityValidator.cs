using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Common.Utility.Interfaces;

public interface IEntityValidator
{
    void EnsureWorkoutExists(Workout workout, Guid workoutId);

    void EnsureExerciseExists(Exercise exercise, Guid exerciseId, Guid workoutId);

    void EnsureSeriesExists(Series series, Guid seriesId, Guid exerciseId);

    void EnsureUserExists(User user, Guid userId);
}