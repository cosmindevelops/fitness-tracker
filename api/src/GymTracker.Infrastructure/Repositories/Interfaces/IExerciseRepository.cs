using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface IExerciseRepository
{
    Task<IEnumerable<Exercise>> GetAllExercisesForWorkoutAsync(Guid workoutId);
    Task<Exercise> GetExerciseByIdAsync(Guid exerciseId);
    Task<Exercise> CreateExerciseAsync(Exercise exercise);
    Task UpdateExerciseAsync(Exercise exercise);
    Task DeleteExerciseAsync(Guid exerciseId);
}