using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface ITemplateExerciseRepository
{
    Task<IEnumerable<TemplateExercise>> GetAllTemplateExercisesAsync();

    Task<TemplateExercise> GetTemplateExerciseByIdAsync(Guid id);

    Task<TemplateExercise> CreateTemplateExerciseAsync(TemplateExercise templateExercise);

    Task UpdateTemplateExerciseAsync(TemplateExercise templateExercise);

    Task DeleteTemplateExerciseAsync(Guid id);
}