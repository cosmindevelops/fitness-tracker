using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility.Interfaces;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Services;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ExerciseService> _logger;
    private readonly IGuidValidator _guidValidator;
    private readonly IEntityValidator _entityValidator;

    public ExerciseService(IExerciseRepository exerciseRepository, IWorkoutRepository workoutRepository, IMapper mapper, ILogger<ExerciseService> logger, IGuidValidator guidValidator, IEntityValidator entityValidator)
    {
        _exerciseRepository = exerciseRepository ?? throw new ArgumentNullException(nameof(exerciseRepository));
        _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _guidValidator = guidValidator ?? throw new ArgumentNullException(nameof(guidValidator));
        _entityValidator = entityValidator ?? throw new ArgumentNullException(nameof(entityValidator));
    }

    public async Task<IEnumerable<ExerciseResponseDto>> GetAllExercisesForWorkoutAsync(Guid userId, Guid workoutId)
    {
        _guidValidator.Validate(userId, workoutId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        _logger.LogInformation("Getting all exercises for workout {WorkoutId}", workoutId);

        var exercises = await _exerciseRepository.GetAllExercisesForWorkoutAsync(workoutId);
        return _mapper.Map<IEnumerable<ExerciseResponseDto>>(exercises);
    }

    public async Task<ExerciseResponseDto> GetExerciseByIdAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        _logger.LogInformation("Getting exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        return _mapper.Map<ExerciseResponseDto>(exercise);
    }

    public async Task<ExerciseResponseDto> CreateExerciseAsync(Guid userId, Guid workoutId, ExerciseCreateDto exerciseDto)
    {
        _guidValidator.Validate(userId, workoutId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        _logger.LogInformation("Creating exercise for workout {WorkoutId}", workoutId);

        var exercise = _mapper.Map<Exercise>(exerciseDto);
        exercise.WorkoutId = workoutId;
        var createdExercise = await _exerciseRepository.CreateExerciseAsync(exercise);
        return _mapper.Map<ExerciseResponseDto>(createdExercise);
    }

    public async Task<Exercise> UpdateExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId, ExerciseUpdateDto exerciseDto)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        _logger.LogInformation("Updating exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        _mapper.Map(exerciseDto, exercise);
        await _exerciseRepository.UpdateExerciseAsync(exercise);
        return exercise;
    }

    public async Task DeleteExerciseAsync(Guid userId, Guid workoutId, Guid exerciseId)
    {
        _guidValidator.Validate(userId, workoutId, exerciseId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        _logger.LogInformation("Deleting exercise {ExerciseId} for workout {WorkoutId}", exerciseId, workoutId);

        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);
        _entityValidator.EnsureExerciseExists(exercise, exerciseId, workoutId);

        await _exerciseRepository.DeleteExerciseAsync(exerciseId);
    }
}