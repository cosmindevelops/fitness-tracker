using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface IWorkoutTemplateRepository
{
    Task<IEnumerable<WorkoutTemplate>> GetAllWorkoutTemplatesDetailedAsync();

    Task<IEnumerable<WorkoutTemplate>> GetAllWorkoutTemplatesBasicAsync();

    Task<WorkoutTemplate> GetByIdAsync(Guid id);

    Task<WorkoutTemplate> GetWorkoutTemplateByIdAsync(Guid templateId);

    Task<WorkoutTemplate> CreateWorkoutTemplateAsync(WorkoutTemplate workoutTemplate);

    Task UpdateWorkoutTemplateAsync(WorkoutTemplate workoutTemplate);

    Task DeleteWorkoutTemplateAsync(Guid id);
}