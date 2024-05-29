using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SeriesRepository> _logger;

    public SeriesRepository(ApplicationDbContext context, ILogger<SeriesRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Series>> GetAllSeriesForExerciseAsync(Guid exerciseId)
    {
        _logger.LogInformation("Getting all series for exercise {ExerciseId}", exerciseId);
        return await _context.Series.Where(s => s.ExerciseId == exerciseId).ToListAsync();
    }

    public async Task<Series> GetSeriesByIdAsync(Guid seriesId)
    {
        _logger.LogInformation("Getting series by id {SeriesId}", seriesId);
        return await _context.Series.FindAsync(seriesId);
    }

    public async Task<Series> CreateSeriesAsync(Series series)
    {
        if (series == null) throw new ArgumentNullException(nameof(series));

        _logger.LogInformation("Creating series for exercise {ExerciseId}", series.ExerciseId);

        await _context.Series.AddAsync(series);
        await _context.SaveChangesAsync();
        return series;
    }

    public async Task UpdateSeriesAsync(Series series)
    {
        if (series == null) throw new ArgumentNullException(nameof(series));

        _logger.LogInformation("Updating series {SeriesId}", series.Id);

        _context.Series.Update(series);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSeriesAsync(Guid seriesId)
    {
        _logger.LogInformation("Deleting series {SeriesId}", seriesId);

        var series = await _context.Series.FindAsync(seriesId);
        if (series != null)
        {
            _context.Series.Remove(series);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing series {SeriesId}", seriesId);
            throw new InvalidOperationException("Series to delete not found.");
        }
    }
}