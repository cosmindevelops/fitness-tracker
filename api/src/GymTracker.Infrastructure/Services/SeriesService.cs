using AutoMapper;
using GymTracker.Core.DTOs;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Repositories.Interfaces;

namespace GymTracker.Infrastructure.Services;

public class SeriesService
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

    public async Task<SeriesResponseDto> CreateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, SeriesCreateDto seriesDto)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            throw new UnauthorizedAccessException("Cannot create a series for a workout that does not belong to the user.");
        }

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            throw new UnauthorizedAccessException("Cannot create a series for an exercise that does not belong to the workout.");
        }

        var series = _mapper.Map<Series>(seriesDto);
        series.ExerciseId = exerciseId;
        var createdSeries = await _seriesRepository.CreateSeriesAsync(series);
        return _mapper.Map<SeriesResponseDto>(createdSeries);
    }

    public async Task<bool> UpdateSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId, SeriesUpdateDto seriesDto)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            return false;
        }

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            return false;
        }

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);
        if (series == null || series.ExerciseId != exerciseId)
        {
            return false;
        }

        _mapper.Map(seriesDto, series);
        await _seriesRepository.UpdateSeriesAsync(series);
        return true;
    }

    public async Task<bool> DeleteSeriesAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            return false;
        }

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            return false;
        }

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);
        if (series == null || series.ExerciseId != exerciseId)
        {
            return false;
        }

        await _seriesRepository.DeleteSeriesAsync(seriesId);
        return true;
    }

    public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesForExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            throw new UnauthorizedAccessException("Cannot access series for a workout that does not belong to the user.");
        }

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            throw new UnauthorizedAccessException("Cannot access series for an exercise that does not belong to the workout.");
        }

        var series = await _seriesRepository.GetAllSeriesForExerciseAsync(exerciseId);
        return _mapper.Map<IEnumerable<SeriesResponseDto>>(series);
    }

    public async Task<SeriesResponseDto> GetSeriesByIdAsync(Guid userId, Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            throw new UnauthorizedAccessException("Cannot access a series for a workout that does not belong to the user.");
        }

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            throw new UnauthorizedAccessException("Cannot access a series for an exercise that does not belong to the workout.");
        }

        var series = await _seriesRepository.GetSeriesByIdAsync(seriesId);
        if (series == null || series.ExerciseId != exerciseId)
        {
            return null;
        }

        return _mapper.Map<SeriesResponseDto>(series);
    }
}