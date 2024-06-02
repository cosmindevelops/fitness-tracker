using AutoMapper;
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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                SlidingExpiration = TimeSpan.FromMinutes(10)
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

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> workouts))
        {
            var workout = workouts.FirstOrDefault(w => w.Id == workoutId);
            if (workout != null)
            {
                _logger.LogInformation("Cache hit for workout {WorkoutId}", workoutId);
                return workout;
            }
        }

        _logger.LogInformation("Cache miss for workout {WorkoutId}", workoutId);
        var workoutEntity = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workoutEntity, workoutId);

        var workoutResponse = _mapper.Map<WorkoutResponseDto>(workoutEntity);

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> workoutList))
        {
            workoutList.Add(workoutResponse);
            _cache.Set($"{CacheKey}{userId}", workoutList);
        }

        return workoutResponse;
    }

    public async Task<IEnumerable<WorkoutResponseDto>> GetWorkoutsByNameAsync(Guid userId, string name)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Getting workouts by name {Name} for user {UserId}", name, userId);

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> workouts))
        {
            var filteredWorkouts = workouts.Where(w => w.Notes.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (filteredWorkouts.Any())
            {
                _logger.LogInformation("Cache hit for workouts by name {Name}", name);
                return filteredWorkouts;
            }
        }

        _logger.LogInformation("Cache miss for workouts by name {Name}", name);
        var workoutEntities = await _workoutRepository.GetWorkoutsByNameAsync(userId, name) ?? new List<Workout>();
        var workoutResponseList = _mapper.Map<IEnumerable<WorkoutResponseDto>>(workoutEntities);

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> cachedWorkouts))
        {
            cachedWorkouts.AddRange(workoutResponseList);
            _cache.Set($"{CacheKey}{userId}", cachedWorkouts);
        }

        return workoutResponseList;
    }

    public async Task<IEnumerable<WorkoutResponseDto>> SearchWorkoutsAsync(Guid userId, string name, DateTime? date)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Searching workouts by name {Name} and date {Date} for user {UserId}", name, date, userId);

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> workouts))
        {
            var filteredWorkouts = workouts.Where(w =>
                (string.IsNullOrEmpty(name) || w.Notes.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                (!date.HasValue || w.Date.Date == date.Value.Date))
                .ToList();

            if (filteredWorkouts.Any())
            {
                _logger.LogInformation("Cache hit for workouts by name {Name} and date {Date}", name, date);
                return filteredWorkouts;
            }
        }

        _logger.LogInformation("Cache miss for workouts by name {Name} and date {Date}", name, date);
        var workoutEntities = await _workoutRepository.SearchWorkoutsAsync(userId, name, date) ?? new List<Workout>();

        // Log the dates being processed
        foreach (var workout in workoutEntities)
        {
            _logger.LogInformation("Workout: {WorkoutId}, Date: {WorkoutDate}, User: {UserId}", workout.Id, workout.Date, userId);
        }

        var workoutResponseList = _mapper.Map<IEnumerable<WorkoutResponseDto>>(workoutEntities);

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> cachedWorkouts))
        {
            cachedWorkouts.AddRange(workoutResponseList);
            _cache.Set($"{CacheKey}{userId}", cachedWorkouts);
        }
        else
        {
            _cache.Set($"{CacheKey}{userId}", workoutResponseList);
        }

        return workoutResponseList;
    }

    public async Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto workoutDto)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Creating workout for user {UserId}", userId);

        workoutDto.Date = DateTime.SpecifyKind(workoutDto.Date, DateTimeKind.Utc);

        var workout = _mapper.Map<Workout>(workoutDto);
        workout.UserId = userId;

        var createdWorkout = await _workoutRepository.CreateWorkoutAsync(workout);

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> workouts))
        {
            var newWorkout = _mapper.Map<WorkoutResponseDto>(createdWorkout);
            workouts.Add(newWorkout);
            _cache.Set($"{CacheKey}{userId}", workouts);
        }

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

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> workouts))
        {
            var updatedWorkout = _mapper.Map<WorkoutResponseDto>(workout);
            var index = workouts.FindIndex(w => w.Id == workoutId);
            if (index != -1)
            {
                workouts[index] = updatedWorkout;
                _cache.Set($"{CacheKey}{userId}", workouts);
            }
        }

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task DeleteWorkoutAsync(Guid userId, Guid workoutId)
    {
        _guidValidator.Validate(userId, workoutId);

        _logger.LogInformation("Deleting workout {WorkoutId} for user {UserId}", workoutId, userId);

        var workout = await _workoutRepository.GetWorkoutByIdAsync(userId, workoutId);
        _entityValidator.EnsureWorkoutExists(workout, workoutId);

        await _workoutRepository.DeleteWorkoutAsync(workoutId);

        if (_cache.TryGetValue($"{CacheKey}{userId}", out List<WorkoutResponseDto> workouts))
        {
            workouts.RemoveAll(w => w.Id == workoutId);
            _cache.Set($"{CacheKey}{userId}", workouts);
        }
    }
}