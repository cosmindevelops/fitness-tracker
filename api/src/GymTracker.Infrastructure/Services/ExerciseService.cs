using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility;
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

    public async Task<IEnumerable<ExerciseResponseDto>> GetAllExercisesForWorkoutAsync(Guid userId, Guid workoutId)
    {
        GuidValidator.Validate(userId, workoutId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercises = await _exerciseRepository.GetAllExercisesForWorkoutAsync(workoutId);
        return _mapper.Map<IEnumerable<ExerciseResponseDto>>(exercises);
    }

    public async Task<ExerciseResponseDto> GetExerciseByIdAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        return _mapper.Map<ExerciseResponseDto>(exercise);
    }

    public async Task<ExerciseResponseDto> CreateExerciseAsync(Guid userId, Guid workoutId, ExerciseCreateDto exerciseDto)
    {
        GuidValidator.Validate(userId, workoutId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = _mapper.Map<Exercise>(exerciseDto);
        exercise.WorkoutId = workoutId;
        var createdExercise = await _exerciseRepository.CreateExerciseAsync(exercise);
        return _mapper.Map<ExerciseResponseDto>(createdExercise);
    }

    public async Task<Exercise> UpdateExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId, ExerciseUpdateDto exerciseDto)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        _mapper.Map(exerciseDto, exercise);
        await _exerciseRepository.UpdateExerciseAsync(exercise);
        return exercise;
    }

    public async Task DeleteExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        GuidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);

        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        EntityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        await _exerciseRepository.DeleteExerciseAsync(exerciseId);
    }
}