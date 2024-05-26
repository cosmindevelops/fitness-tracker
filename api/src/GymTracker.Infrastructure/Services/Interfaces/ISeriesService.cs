using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;

namespace GymTracker.Infrastructure.Services.Interfaces;

public interface ISeriesService
{
    Task<IEnumerable<SeriesResponseDto>> GetAllSeriesForExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId);

    Task<SeriesResponseDto> GetSeriesByIdAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId);

    Task<SeriesResponseDto> CreateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, SeriesCreateDto seriesDto);

    Task<Series> UpdateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId, SeriesUpdateDto seriesDto);

    Task DeleteSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId);
}