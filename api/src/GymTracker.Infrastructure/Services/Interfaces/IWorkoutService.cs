using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IWorkoutService
{
    Task<IEnumerable<WorkoutResponseDto>> GetAllWorkoutsForUserAsync(Guid userId);

    Task<WorkoutResponseDto> GetWorkoutByIdForUserAsync(Guid userId, Guid workoutId);

    Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto workoutDto);

    Task<Workout> UpdateWorkoutAsync(Guid userId, Guid workoutId, WorkoutUpdateDto workoutDto);

    Task DeleteWorkoutAsync(Guid userId, Guid workoutId);
}