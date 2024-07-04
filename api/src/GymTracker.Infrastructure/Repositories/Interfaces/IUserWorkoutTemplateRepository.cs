using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface IUserWorkoutTemplateRepository
{
    Task<IEnumerable<UserWorkoutTemplate>> GetUserWorkoutTemplatesByUserIdAsync(Guid userId);

    Task<UserWorkoutTemplate> GetUserWorkoutTemplateByIdAsync(Guid templateId);

    Task<UserWorkoutTemplate> CreateUserWorkoutTemplateAsync(UserWorkoutTemplate userWorkoutTemplate);

    Task UpdateUserWorkoutTemplateAsync(UserWorkoutTemplate userWorkoutTemplate);

    Task DeleteUserWorkoutTemplateAsync(Guid templateId);

    Task<UserWorkoutTemplate> GetUserWorkoutTemplateByUserIdAndTemplateIdAsync(Guid userId, Guid templateId);
}