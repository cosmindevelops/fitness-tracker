using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class WorkoutTemplateRepository : IWorkoutTemplateRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WorkoutTemplateRepository> _logger;

    public WorkoutTemplateRepository(ApplicationDbContext context, ILogger<WorkoutTemplateRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<WorkoutTemplate>> GetAllWorkoutTemplatesBasicAsync()
    {
        _logger.LogInformation("Getting all basic workout templates");
        var templates = await _context.WorkoutTemplates
                             .Select(wt => new WorkoutTemplate
                             {
                                 Id = wt.Id,
                                 Name = wt.Name,
                                 DurationWeeks = wt.DurationWeeks,
                                 Description = wt.Description,
                                 TemplateWeeks = new List<TemplateWeek>()
                             })
                             .ToListAsync();

        _logger.LogInformation("Retrieved {Count} basic templates", templates.Count);
        return templates;
    }

    public async Task<IEnumerable<WorkoutTemplate>> GetAllWorkoutTemplatesDetailedAsync()
    {
        _logger.LogInformation("Getting all detailed workout templates");
        var templates = await _context.WorkoutTemplates
                             .Include(wt => wt.TemplateWeeks)
                                 .ThenInclude(tw => tw.TemplateWorkouts)
                                     .ThenInclude(tw => tw.TemplateExercises)
                             .ToListAsync();

        _logger.LogInformation("Retrieved {Count} detailed templates", templates.Count);
        return templates;
    }

    public async Task<WorkoutTemplate> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting workout template by id {Id}", id);
        return await _context.WorkoutTemplates
                             .Include(wt => wt.TemplateWeeks)
                             .ThenInclude(tw => tw.TemplateWorkouts)
                             .ThenInclude(tw => tw.TemplateExercises)
                             .FirstOrDefaultAsync(wt => wt.Id == id);
    }

    public async Task<WorkoutTemplate> GetWorkoutTemplateByIdAsync(Guid templateId)
    {
        _logger.LogInformation("Getting workout template by id {TemplateId}", templateId);
        return await _context.WorkoutTemplates
                             .FirstOrDefaultAsync(wt => wt.Id == templateId);
    }

    public async Task<WorkoutTemplate> CreateWorkoutTemplateAsync(WorkoutTemplate workoutTemplate)
    {
        if (workoutTemplate == null) throw new ArgumentNullException(nameof(workoutTemplate));

        _logger.LogInformation("Creating new workout template {TemplateName}", workoutTemplate.Name);

        await _context.WorkoutTemplates.AddAsync(workoutTemplate);
        await _context.SaveChangesAsync();
        return workoutTemplate;
    }

    public async Task UpdateWorkoutTemplateAsync(WorkoutTemplate workoutTemplate)
    {
        if (workoutTemplate == null) throw new ArgumentNullException(nameof(workoutTemplate));

        _logger.LogInformation("Updating workout template {TemplateId}", workoutTemplate.Id);

        _context.WorkoutTemplates.Update(workoutTemplate);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkoutTemplateAsync(Guid templateId)
    {
        _logger.LogInformation("Deleting workout template {TemplateId}", templateId);

        var workoutTemplate = await _context.WorkoutTemplates.FindAsync(templateId);
        if (workoutTemplate != null)
        {
            _context.WorkoutTemplates.Remove(workoutTemplate);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing workout template {TemplateId}", templateId);
            throw new InvalidOperationException("Workout template to delete not found");
        }
    }
}