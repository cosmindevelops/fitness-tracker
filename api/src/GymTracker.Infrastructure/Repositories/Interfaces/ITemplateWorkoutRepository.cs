using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface ITemplateWorkoutRepository
{
    Task<IEnumerable<TemplateWorkout>> GetAllAsync();

    Task<TemplateWorkout> GetByIdAsync(Guid id);

    Task<IEnumerable<TemplateWorkout>> GetAllTemplateWorkoutsAsync();

    Task<TemplateWorkout> GetTemplateWorkoutByIdAsync(Guid id);

    Task<TemplateWorkout> CreateTemplateWorkoutAsync(TemplateWorkout templateWorkout);

    Task UpdateTemplateWorkoutAsync(TemplateWorkout templateWorkout);

    Task DeleteTemplateWorkoutAsync(Guid id);
}