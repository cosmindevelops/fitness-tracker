using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;

namespace GymTracker.Infrastructure.Services;

public class SeriesService : ISeriesService
{
    private readonly ISeriesRepository _seriesRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IMapper _mapper;

    public SeriesService(ISeriesRepository seriesRepository, IExerciseRepository exerciseRepository, IWorkoutRepository workoutRepository, IMapper mapper)
    {
        _seriesRepository = seriesRepository;
        _exerciseRepository = exerciseRepository;
        _workoutRepository = workoutRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesForExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        var series = await _seriesRepository.GetAllSeriesForExerciseAsync(exerciseId);
        return _mapper.Map<IEnumerable<SeriesResponseDto>>(series);
    }

    public async Task<SeriesResponseDto> GetSeriesByIdAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId, seriesId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);

        EntityValidator.EnsureSeriesListExists(series, seriesId, exerciseId);

        return _mapper.Map<SeriesResponseDto>(series);
    }

    public async Task<SeriesResponseDto> CreateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, SeriesCreateDto seriesDto)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        var series = _mapper.Map<Series>(seriesDto);
        series.ExerciseId = exerciseId;
        var createdSeries = await _seriesRepository.CreateSeriesAsync(series);
        return _mapper.Map<SeriesResponseDto>(createdSeries);
    }

    public async Task<Series> UpdateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId, SeriesUpdateDto seriesDto)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId, seriesId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);

        EntityValidator.EnsureSeriesListExists(series, seriesId, exerciseId);

        _mapper.Map(seriesDto, series);
        await _seriesRepository.UpdateSeriesAsync(series);
        return series;
    }

    public async Task DeleteSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId, seriesId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);

        EntityValidator.EnsureSeriesListExists(series, seriesId, exerciseId);

        await _seriesRepository.DeleteSeriesAsync(seriesId);
    }
}