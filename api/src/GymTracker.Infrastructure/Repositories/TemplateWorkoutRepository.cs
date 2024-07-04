using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class TemplateWorkoutRepository : ITemplateWorkoutRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplateWorkoutRepository> _logger;

    public TemplateWorkoutRepository(ApplicationDbContext context, ILogger<TemplateWorkoutRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TemplateWorkout>> GetAllAsync()
    {
        _logger.LogInformation("Getting all template workouts");
        return await _context.TemplateWorkouts
                             .Include(tw => tw.TemplateExercises)
                             .ToListAsync();
    }

    public async Task<TemplateWorkout> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting template workout by id {Id}", id);
        return await _context.TemplateWorkouts
                             .Include(tw => tw.TemplateExercises)
                             .FirstOrDefaultAsync(tw => tw.Id == id);
    }

    public async Task<IEnumerable<TemplateWorkout>> GetAllTemplateWorkoutsAsync()
    {
        _logger.LogInformation("Getting all template workouts");
        return await _context.TemplateWorkouts
                             .ToListAsync();
    }

    public async Task<TemplateWorkout> GetTemplateWorkoutByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting template workout by id {Id}", id);
        return await _context.TemplateWorkouts
                             .FirstOrDefaultAsync(tw => tw.Id == id);
    }

    public async Task<TemplateWorkout> CreateTemplateWorkoutAsync(TemplateWorkout templateWorkout)
    {
        if (templateWorkout == null) throw new ArgumentNullException(nameof(templateWorkout));

        _logger.LogInformation("Creating template workout for template week {WeekId}", templateWorkout.TemplateWeekId);

        await _context.TemplateWorkouts.AddAsync(templateWorkout);
        await _context.SaveChangesAsync();
        return templateWorkout;
    }

    public async Task UpdateTemplateWorkoutAsync(TemplateWorkout templateWorkout)
    {
        if (templateWorkout == null) throw new ArgumentNullException(nameof(templateWorkout));

        _logger.LogInformation("Updating template workout {WorkoutId}", templateWorkout.Id);

        _context.TemplateWorkouts.Update(templateWorkout);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTemplateWorkoutAsync(Guid workoutId)
    {
        _logger.LogInformation("Deleting template workout {WorkoutId}", workoutId);

        var templateWorkout = await _context.TemplateWorkouts.FindAsync(workoutId);
        if (templateWorkout != null)
        {
            _context.TemplateWorkouts.Remove(templateWorkout);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing template workout {WorkoutId}", workoutId);
            throw new InvalidOperationException("Template workout to delete not found.");
        }
    }
}