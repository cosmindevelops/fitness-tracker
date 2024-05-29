using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility.Interfaces;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Services;

public class SeriesService : ISeriesService
{
    private readonly ISeriesRepository _seriesRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SeriesService> _logger;
    private readonly IGuidValidator _guidValidator;
    private readonly IEntityValidator _entityValidator;

    public SeriesService(ISeriesRepository seriesRepository, IExerciseRepository exerciseRepository, IWorkoutRepository workoutRepository, IMapper mapper, ILogger<SeriesService> logger, IGuidValidator guidValidator, IEntityValidator entityValidator)
    {
        _seriesRepository = seriesRepository ?? throw new ArgumentNullException(nameof(seriesRepository));
        _exerciseRepository = exerciseRepository ?? throw new ArgumentNullException(nameof(exerciseRepository));
        _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _guidValidator = guidValidator ?? throw new ArgumentNullException(nameof(guidValidator));
        _entityValidator = entityValidator ?? throw new ArgumentNullException(nameof(entityValidator));
    }

    public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesForExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        _logger.LogInformation("Getting all series for exercise {ExerciseId}", exerciseId);

        var series = await _seriesRepository.GetAllSeriesForExerciseAsync(exerciseId);
        return _mapper.Map<IEnumerable<SeriesResponseDto>>(series);
    }

    public async Task<SeriesResponseDto> GetSeriesByIdAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId, seriesId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        _logger.LogInformation("Getting series {SeriesId} for exercise {ExerciseId}", seriesId, exerciseId);

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);
        _entityValidator.EnsureSeriesExists(series, seriesId, exerciseId);

        return _mapper.Map<SeriesResponseDto>(series);
    }

    public async Task<SeriesResponseDto> CreateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, SeriesCreateDto seriesDto)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        _logger.LogInformation("Creating series for exercise {ExerciseId}", exerciseId);

        var series = _mapper.Map<Series>(seriesDto);
        series.ExerciseId = exerciseId;
        var createdSeries = await _seriesRepository.CreateSeriesAsync(series);
        return _mapper.Map<SeriesResponseDto>(createdSeries);
    }

    public async Task<Series> UpdateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId, SeriesUpdateDto seriesDto)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId, seriesId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        _logger.LogInformation("Updating series {SeriesId} for exercise {ExerciseId}", seriesId, exerciseId);

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);
        _entityValidator.EnsureSeriesExists(series, seriesId, exerciseId);

        _mapper.Map(seriesDto, series);
        await _seriesRepository.UpdateSeriesAsync(series);
        return series;
    }

    public async Task DeleteSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId, seriesId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        _logger.LogInformation("Deleting series {SeriesId} for exercise {ExerciseId}", seriesId, exerciseId);

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);
        _entityValidator.EnsureSeriesExists(series, seriesId, exerciseId);

        await _seriesRepository.DeleteSeriesAsync(seriesId);
    }
}