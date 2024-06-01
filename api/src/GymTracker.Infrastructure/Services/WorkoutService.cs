﻿using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility.Interfaces;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<WorkoutService> _logger;
    private readonly IGuidValidator _guidValidator;
    private readonly IEntityValidator _entityValidator;
    private readonly IMemoryCache _cache;
    private static readonly string CacheKey = "WorkoutsCache_";

    public WorkoutService(IWorkoutRepository workoutRepository, IMapper mapper, ILogger<WorkoutService> logger, IGuidValidator guidValidator, IEntityValidator entityValidator, IMemoryCache cache)
    {
        _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _guidValidator = guidValidator ?? throw new ArgumentNullException(nameof(guidValidator));
        _entityValidator = entityValidator ?? throw new ArgumentNullException(nameof(entityValidator));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<WorkoutResponseDto>> GetAllWorkoutsForUserAsync(Guid userId)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Getting all workouts for user {UserId}", userId);

        if (!_cache.TryGetValue($"{CacheKey}{userId}", out IEnumerable<WorkoutResponseDto> workouts))
        {
            _logger.LogInformation("Cache miss for user {UserId}", userId);

            var workoutEntities = await _workoutRepository.GetAllWorkoutsAsync(userId) ?? new List<Workout>();
            workouts = _mapper.Map<IEnumerable<WorkoutResponseDto>>(workoutEntities);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _cache.Set($"{CacheKey}{userId}", workouts, cacheEntryOptions);
        }
        else
        {
            _logger.LogInformation("Cache hit for user {UserId}", userId);
        }

        return workouts;
    }

    public async Task<WorkoutResponseDto> GetWorkoutByIdForUserAsync(Guid userId, Guid workoutId)
    {
        _guidValidator.Validate(userId, workoutId);

        _logger.LogInformation("Getting workout {WorkoutId} for user {UserId}", workoutId, userId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task<IEnumerable<WorkoutResponseDto>> GetWorkoutsByNameAsync(Guid userId, string name)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Getting workouts by name {Name} for user {UserId}", name, userId);

        var workouts = await _workoutRepository.GetWorkoutsByNameAsync(userId, name) ?? new List<Workout>();
        return _mapper.Map<IEnumerable<WorkoutResponseDto>>(workouts);
    }

    public async Task<IEnumerable<WorkoutResponseDto>> SearchWorkoutsAsync(Guid userId, string name, DateTime? date)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Searching workouts by name {Name} and date {Date} for user {UserId}", name, date, userId);

        var workouts = await _workoutRepository.SearchWorkoutsAsync(userId, name, date) ?? new List<Workout>();
        return _mapper.Map<IEnumerable<WorkoutResponseDto>>(workouts);
    }

    public async Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto workoutDto)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Creating workout for user {UserId}", userId);

        var workout = _mapper.Map<Workout>(workoutDto);
        workout.UserId = userId;
        var createdWorkout = await _workoutRepository.CreateWorkoutAsync(workout);

        // Invalidate the cache
        _cache.Remove($"{CacheKey}{userId}");

        return _mapper.Map<WorkoutResponseDto>(createdWorkout);
    }

    public async Task<WorkoutResponseDto> UpdateWorkoutAsync(Guid userId, Guid workoutId, WorkoutUpdateDto workoutDto)
    {
        _guidValidator.Validate(userId, workoutId);

        _logger.LogInformation("Updating workout {WorkoutId} for user {UserId}", workoutId, userId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        _mapper.Map(workoutDto, workout);
        await _workoutRepository.UpdateWorkoutAsync(workout);

        // Invalidate the cache
        _cache.Remove($"{CacheKey}{userId}");

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task DeleteWorkoutAsync(Guid userId, Guid workoutId)
    {
        _guidValidator.Validate(userId, workoutId);

        _logger.LogInformation("Deleting workout {WorkoutId} for user {UserId}", workoutId, userId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        await _workoutRepository.DeleteWorkoutAsync(workoutId);

        // Invalidate the cache
        _cache.Remove($"{CacheKey}{userId}");
    }
}