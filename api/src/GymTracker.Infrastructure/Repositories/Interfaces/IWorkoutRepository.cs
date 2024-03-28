using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface IWorkoutRepository
{
    Task<IEnumerable<Workout>> GetAllWorkoutsAsync();

    Task<Workout> GetWorkoutByIdAsync(Guid workoutId);

    Task<Workout> CreateWorkoutAsync(Workout workout);

    Task UpdateWorkoutAsync(Workout workout);

    Task DeleteWorkoutAsync(Guid workoutId);
}