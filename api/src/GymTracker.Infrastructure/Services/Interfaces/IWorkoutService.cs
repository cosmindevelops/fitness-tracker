using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IWorkoutService
{
    Task<IEnumerable<WorkoutResponseDto>> GetAllWorkoutsForUserAsync(Guid userId);

    Task<WorkoutResponseDto> GetWorkoutByIdForUserAsync(Guid userId, Guid workoutId);

    Task<IEnumerable<WorkoutResponseDto>> GetWorkoutsByNameAsync(Guid userId, string name);

    Task<IEnumerable<WorkoutResponseDto>> SearchWorkoutsAsync(Guid userId, string name, DateTime? date);

    Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto workoutDto);

    Task<WorkoutResponseDto> UpdateWorkoutAsync(Guid userId, Guid workoutId, WorkoutUpdateDto workoutDto);

    Task DeleteWorkoutAsync(Guid userId, Guid workoutId);
}