using AutoMapper;
using GymTracker.Core.DTOs;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Repositories.Interfaces;

namespace GymTracker.Infrastructure.Services;

public class ExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IMapper _mapper;

    public ExerciseService(IExerciseRepository exerciseRepository, IWorkoutRepository workoutRepository, IMapper mapper)
    {
        _exerciseRepository = exerciseRepository;
        _workoutRepository = workoutRepository;
        _mapper = mapper;
    }

    public async Task<ExerciseResponseDto> CreateExerciseAsync(Guid userId, Guid workoutId, ExerciseCreateDto exerciseDto)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            throw new UnauthorizedAccessException("Cannot create an exercise for a workout that does not belong to the user.");
        }

        var exercise = _mapper.Map<Exercise>(exerciseDto);
        exercise.WorkoutId = workoutId;
        var createdExercise = await _exerciseRepository.CreateExerciseAsync(exercise);
        return _mapper.Map<ExerciseResponseDto>(createdExercise);
    }

    public async Task<bool> UpdateExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId, ExerciseUpdateDto exerciseDto)
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

        _mapper.Map(exerciseDto, exercise);
        await _exerciseRepository.UpdateExerciseAsync(exercise);
        return true;
    }

    public async Task<bool> DeleteExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId)
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

        await _exerciseRepository.DeleteExerciseAsync(exerciseId);
        return true;
    }

    public async Task<IEnumerable<ExerciseResponseDto>> GetAllExercisesForWorkoutAsync(Guid userId, Guid workoutId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            throw new UnauthorizedAccessException("Cannot access exercises for a workout that does not belong to the user.");
        }

        var exercises = await _exerciseRepository.GetAllExercisesForWorkoutAsync(workoutId);
        return _mapper.Map<IEnumerable<ExerciseResponseDto>>(exercises);
    }

    public async Task<ExerciseResponseDto> GetExerciseByIdAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            return null;
        }

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        if (exercise == null || exercise.WorkoutId != workoutId)
        {
            return null;
        }

        return _mapper.Map<ExerciseResponseDto>(exercise);
    }
}