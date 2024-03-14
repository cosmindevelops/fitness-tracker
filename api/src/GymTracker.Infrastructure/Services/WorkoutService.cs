using AutoMapper;
using GymTracker.Core.DTOs;
using GymTracker.Core.Entities;
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

    public async Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto workoutDto)
    {
        var workout = _mapper.Map<Workout>(workoutDto);
        workout.UserId = userId;
        var createdWorkout = await _workoutRepository.CreateWorkoutAsync(workout);
        return _mapper.Map<WorkoutResponseDto>(createdWorkout);
    }

    public async Task<bool> UpdateWorkoutAsync(Guid userId, Guid workoutId, WorkoutUpdateDto workoutDto)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            return false;
        }

        _mapper.Map(workoutDto, workout);
        await _workoutRepository.UpdateWorkoutAsync(workout);
        return true;
    }

    public async Task<bool> DeleteWorkoutAsync(Guid userId, Guid workoutId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout == null || workout.UserId != userId)
        {
            return false;
        }

        await _workoutRepository.DeleteWorkoutAsync(workoutId);
        return true;
    }

    public async Task<IEnumerable<WorkoutResponseDto>> GetAllWorkoutsForUserAsync(Guid userId)
    {
        var workouts = await _workoutRepository.GetAllWorkoutsAsync();
        var userWorkouts = workouts.Where(w => w.UserId == userId);
        return _mapper.Map<IEnumerable<WorkoutResponseDto>>(userWorkouts);
    }

    public async Task<WorkoutResponseDto> GetWorkoutByIdForUserAsync(Guid workoutId, Guid userId)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId);
        if (workout != null && workout.UserId == userId)
        {
            return _mapper.Map<WorkoutResponseDto>(workout);
        }
        else
        {
            return null;
        }
    }
}
