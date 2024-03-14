using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Infrastructure.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly ApplicationDbContext _context;

    public SeriesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Series>> GetAllSeriesForExerciseAsync(Guid exerciseId)
    {
        return await _context.Series.Where(s => s.ExerciseId == exerciseId).ToListAsync();
    }

    public async Task<Series> GetSeriesByIdAsync(Guid seriesId)
    {
        return await _context.Series.FindAsync(seriesId);
    }

    public async Task<Series> CreateSeriesAsync(Series series)
    {
        await _context.Series.AddAsync(series);
        await _context.SaveChangesAsync();
        return series;
    }

    public async Task UpdateSeriesAsync(Series series)
    {
        _context.Series.Update(series);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSeriesAsync(Guid seriesId)
    {
        var series = await _context.Series.FindAsync(seriesId);
        if (series != null)
        {
            _context.Series.Remove(series);
            await _context.SaveChangesAsync();
        }
    }
}
