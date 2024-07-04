using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface IWorkoutTemplateService
{
    Task<IEnumerable<WorkoutTemplateResponseDto>> GetAllTemplatesAsync(bool includeDetails = false);

    Task<WorkoutTemplateResponseDto> GetTemplateByIdWithDetailsAsync(Guid id);

    Task<WorkoutTemplateResponseDto> GetTemplateByIdAsync(Guid id);

    Task<WorkoutTemplateResponseDto> CreateWorkoutTemplateFromJsonFileAsync(string filePath);

    Task DeleteWorkoutTemplateAsync(Guid templateId);

    Task UpdateTemplateAsync(Guid id, WorkoutTemplateUpdateDto updateDto);
}