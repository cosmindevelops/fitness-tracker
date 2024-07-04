using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class TemplateWeekRepository : ITemplateWeekRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplateWeekRepository> _logger;

    public TemplateWeekRepository(ApplicationDbContext context, ILogger<TemplateWeekRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TemplateWeek>> GetAllAsync()
    {
        _logger.LogInformation("Getting all template weeks");
        return await _context.TemplateWeeks
                             .Include(tw => tw.TemplateWorkouts)
                             .ThenInclude(tw => tw.TemplateExercises)
                             .ToListAsync();
    }

    public async Task<TemplateWeek> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting template week by id {Id}", id);
        return await _context.TemplateWeeks
                             .Include(tw => tw.TemplateWorkouts)
                             .ThenInclude(tw => tw.TemplateExercises)
                             .FirstOrDefaultAsync(tw => tw.Id == id);
    }

    public async Task<IEnumerable<TemplateWeek>> GetAllTemplateWeeksAsync()
    {
        _logger.LogInformation("Getting all template weeks");
        return await _context.TemplateWeeks
                             .ToListAsync();
    }

    public async Task<TemplateWeek> GetTemplateWeekByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting template week by id {Id}", id);
        return await _context.TemplateWeeks
                             .FirstOrDefaultAsync(tw => tw.Id == id);
    }

    public async Task<TemplateWeek> CreateTemplateWeekAsync(TemplateWeek templateWeek)
    {
        if (templateWeek == null) throw new ArgumentNullException(nameof(templateWeek));

        _logger.LogInformation("Creating template week for workout template {TemplateId}", templateWeek.WorkoutTemplateId);

        await _context.TemplateWeeks.AddAsync(templateWeek);
        await _context.SaveChangesAsync();
        return templateWeek;
    }

    public async Task UpdateTemplateWeekAsync(TemplateWeek templateWeek)
    {
        if (templateWeek == null) throw new ArgumentNullException(nameof(templateWeek));

        _logger.LogInformation("Updating template week {WeekId}", templateWeek.Id);

        _context.TemplateWeeks.Update(templateWeek);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTemplateWeekAsync(Guid weekId)
    {
        _logger.LogInformation("Deleting template week {WeekId}", weekId);

        var templateWeek = await _context.TemplateWeeks.FindAsync(weekId);
        if (templateWeek != null)
        {
            _context.TemplateWeeks.Remove(templateWeek);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing template week {WeekId}", weekId);
            throw new InvalidOperationException("Template week to delete not found.");
        }
    }
}