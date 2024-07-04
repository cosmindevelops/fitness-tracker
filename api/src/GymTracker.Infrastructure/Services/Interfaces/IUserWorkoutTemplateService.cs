using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IUserWorkoutTemplateService
{
    Task<UserWorkoutTemplateResponseDto> SelectTemplateForUserAsync(Guid userId, Guid templateId, DateTime startDate);

    Task<IEnumerable<UserWorkoutTemplateResponseDto>> GetUserWorkoutTemplatesAsync(Guid userId);

    Task<UserWorkoutTemplateResponseDto> GetUserWorkoutTemplateByIdAsync(Guid userId, Guid userWorkoutTemplateId);

    Task RemoveTemplateForUserAsync(Guid userId, Guid userWorkoutTemplateId);
}