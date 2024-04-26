using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility;
using GymTracker.Infrastructure.Repositories.Interfaces;

namespace GymTracker.Infrastructure.Services;

public class WorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IMapper _mapper;

    public WorkoutService(IWorkoutRepository workoutRepository, IMapper mapper)
    {
        _workoutRepository = workoutRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WorkoutResponseDto>> GetAllWorkoutsForUserAsync(Guid userId)
    {
        GuidValidator.Validate(userId);

        var workouts = await _workoutRepository.GetAllWorkoutsAsync(userId);
        if (workouts == null)
        {
            return new List<WorkoutResponseDto>();
        }

        return _mapper.Map<IEnumerable<WorkoutResponseDto>>(workouts);
    }

    public async Task<WorkoutResponseDto> GetWorkoutByIdForUserAsync(Guid userId, Guid workoutId)
    {
        GuidValidator.Validate(userId, workoutId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto workoutDto)
    {
        GuidValidator.Validate(userId);

        var workout = _mapper.Map<Workout>(workoutDto);
        workout.UserId = userId;
        var createdWorkout = await _workoutRepository.CreateWorkoutAsync(workout);
        return _mapper.Map<WorkoutResponseDto>(createdWorkout);
    }

    public async Task<Workout> UpdateWorkoutAsync(Guid userId, Guid workoutId, WorkoutUpdateDto workoutDto)
    {
        GuidValidator.Validate(userId, workoutId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        _mapper.Map(workoutDto, workout);
        await _workoutRepository.UpdateWorkoutAsync(workout);
        return workout;
    }

    public async Task DeleteWorkoutAsync(Guid userId, Guid workoutId)
    {
        GuidValidator.Validate(userId, workoutId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        EntityValidator.EnsureWorkoutExists(workout, workoutId);

        await _workoutRepository.DeleteWorkoutAsync(workoutId);
    }
}