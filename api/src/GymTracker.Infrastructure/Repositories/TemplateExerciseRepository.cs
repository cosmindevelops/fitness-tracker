using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class TemplateExerciseRepository : ITemplateExerciseRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplateExerciseRepository> _logger;

    public TemplateExerciseRepository(ApplicationDbContext context, ILogger<TemplateExerciseRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TemplateExercise>> GetAllTemplateExercisesAsync()
    {
        _logger.LogInformation("Getting all template exercises");
        return await _context.TemplateExercises.ToListAsync();
    }

    public async Task<TemplateExercise> GetTemplateExerciseByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting template exercise by id {Id}", id);
        return await _context.TemplateExercises.FirstOrDefaultAsync(te => te.Id == id);
    }

    public async Task<TemplateExercise> CreateTemplateExerciseAsync(TemplateExercise templateExercise)
    {
        if (templateExercise == null) throw new ArgumentNullException(nameof(templateExercise));

        _logger.LogInformation("Creating template exercise for template workout {WorkoutId}", templateExercise.TemplateWorkoutId);

        await _context.TemplateExercises.AddAsync(templateExercise);
        await _context.SaveChangesAsync();
        return templateExercise;
    }

    public async Task UpdateTemplateExerciseAsync(TemplateExercise templateExercise)
    {
        if (templateExercise == null) throw new ArgumentNullException(nameof(templateExercise));

        _logger.LogInformation("Updating template exercise {ExerciseId}", templateExercise.Id);

        _context.TemplateExercises.Update(templateExercise);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTemplateExerciseAsync(Guid exerciseId)
    {
        _logger.LogInformation("Deleting template exercise {ExerciseId}", exerciseId);

        var templateExercise = await _context.TemplateExercises.FindAsync(exerciseId);
        if (templateExercise != null)
        {
            _context.TemplateExercises.Remove(templateExercise);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing template exercise {ExerciseId}", exerciseId);
            throw new InvalidOperationException("Template exercise to delete not found.");
        }
    }
}