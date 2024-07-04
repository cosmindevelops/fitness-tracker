using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IUserExerciseProgressService
{
    Task<UserExerciseProgressResponseDto> LogExerciseProgressAsync(Guid userWorkoutTemplateId, Guid templateExerciseId, UserExerciseProgressUpdateDto progressDto);

    Task<UserExerciseProgressResponseDto> UpdateExerciseProgressAsync(Guid progressId, UserExerciseProgressUpdateDto progressDto);

    Task<UserExerciseProgressResponseDto> MarkWorkoutCompletedAsync(Guid progressId, bool completed);

    Task ResetExerciseProgressAsync(Guid progressId);

    Task<IEnumerable<UserExerciseProgressResponseDto>> GetUserExerciseProgressByUserWorkoutTemplateIdAsync(Guid userWorkoutTemplateId);

    Task<UserExerciseProgressResponseDto> GetUserExerciseProgressByIdAsync(Guid progressId);
}