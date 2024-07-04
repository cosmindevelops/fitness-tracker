using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface IUserExerciseProgressRepository
{
    Task<IEnumerable<UserExerciseProgress>> GetUserExerciseProgressByUserWorkoutTemplateIdAsync(Guid userWorkoutTemplateId);

    Task<UserExerciseProgress> GetUserExerciseProgressByIdAsync(Guid progressId);

    Task<UserExerciseProgress> CreateUserExerciseProgressAsync(UserExerciseProgress userExerciseProgress);

    Task UpdateUserExerciseProgressAsync(UserExerciseProgress userExerciseProgress);

    Task DeleteUserExerciseProgressAsync(Guid progressId);
}