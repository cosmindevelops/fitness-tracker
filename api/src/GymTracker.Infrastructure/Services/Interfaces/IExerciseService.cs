using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IExerciseService
{
    Task<IEnumerable<ExerciseResponseDto>> GetAllExercisesForWorkoutAsync(Guid userId, Guid workoutId);

    Task<ExerciseResponseDto> GetExerciseByIdAsync(Guid userId, Guid workoutId, Guid exerciseId);

    Task<ExerciseResponseDto> CreateExerciseAsync(Guid userId, Guid workoutId, ExerciseCreateDto exerciseDto);

    Task<Exercise> UpdateExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId, ExerciseUpdateDto exerciseDto);

    Task DeleteExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId);
}