using GymTracker.Core.Entities;

namespace GymTracker.Infrastructure.Repositories.Interfaces;

public interface ISeriesRepository
{
    Task<IEnumerable<Series>> GetAllSeriesForExerciseAsync(Guid exerciseId);

    Task<Series> GetSeriesByIdAsync(Guid seriesId);

    Task<Series> CreateSeriesAsync(Series series);

    Task UpdateSeriesAsync(Series series);

    Task DeleteSeriesAsync(Guid seriesId);
}