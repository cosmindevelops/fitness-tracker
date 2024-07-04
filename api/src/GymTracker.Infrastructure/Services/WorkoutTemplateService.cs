using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Data;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GymTracker.Infrastructure.Services;

public class WorkoutTemplateService : IWorkoutTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly IWorkoutTemplateRepository _workoutTemplateRepository;
    private readonly ITemplateWeekRepository _templateWeekRepository;
    private readonly ITemplateWorkoutRepository _templateWorkoutRepository;
    private readonly ITemplateExerciseRepository _templateExerciseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<WorkoutTemplateService> _logger;
    private readonly IMemoryCache _cache;

    public WorkoutTemplateService(
        ApplicationDbContext context,
        IWorkoutTemplateRepository workoutTemplateRepository,
        ITemplateWeekRepository templateWeekRepository,
        ITemplateWorkoutRepository templateWorkoutRepository,
        ITemplateExerciseRepository templateExerciseRepository,
        ILogger<WorkoutTemplateService> logger,
        IMapper mapper,
        IMemoryCache cache)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _workoutTemplateRepository = workoutTemplateRepository ?? throw new ArgumentNullException(nameof(workoutTemplateRepository));
        _templateWeekRepository = templateWeekRepository ?? throw new ArgumentNullException(nameof(templateWeekRepository));
        _templateWorkoutRepository = templateWorkoutRepository ?? throw new ArgumentNullException(nameof(templateWorkoutRepository));
        _templateExerciseRepository = templateExerciseRepository ?? throw new ArgumentNullException(nameof(templateExerciseRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache;
    }

    public async Task<IEnumerable<WorkoutTemplateResponseDto>> GetAllTemplatesAsync(bool includeDetails = false)
    {
        string cacheKey = includeDetails ? "AllTemplatesDetailed" : "AllTemplatesBasic";

        if (!_cache.TryGetValue(cacheKey, out IEnumerable<WorkoutTemplateResponseDto> templates))
        {
            _logger.LogInformation("Cache miss. Getting all workout templates from database. Include details: {IncludeDetails}", includeDetails);

            IEnumerable<WorkoutTemplate> dbTemplates;
            if (includeDetails)
            {
                dbTemplates = await _workoutTemplateRepository.GetAllWorkoutTemplatesDetailedAsync();
            }
            else
            {
                dbTemplates = await _workoutTemplateRepository.GetAllWorkoutTemplatesBasicAsync();
            }

            templates = _mapper.Map<IEnumerable<WorkoutTemplateResponseDto>>(dbTemplates);

            if (templates.Any())
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, templates, cacheEntryOptions);
            }
        }
        else
        {
            _logger.LogInformation("Cache hit. Returning all workout templates from cache. Include details: {IncludeDetails}", includeDetails);
        }

        return templates ?? Enumerable.Empty<WorkoutTemplateResponseDto>();
    }

    public async Task<WorkoutTemplateResponseDto> GetTemplateByIdWithDetailsAsync(Guid id)
    {
        string cacheKey = $"TemplateWithDetails_{id}";
        if (!_cache.TryGetValue(cacheKey, out WorkoutTemplateResponseDto template))
        {
            _logger.LogInformation("Cache miss. Getting workout template by id {Id} with details from database", id);
            var dbTemplate = await _workoutTemplateRepository.GetByIdAsync(id);
            if (dbTemplate == null)
            {
                throw new WorkoutTemplateNotFoundException(id);
            }
            template = _mapper.Map<WorkoutTemplateResponseDto>(dbTemplate);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, template, cacheEntryOptions);
        }
        else
        {
            _logger.LogInformation("Cache hit. Returning workout template by id {Id} with details from cache", id);
        }

        return template;
    }

    public async Task<WorkoutTemplateResponseDto> GetTemplateByIdAsync(Guid id)
    {
        string cacheKey = $"Template_{id}";
        if (!_cache.TryGetValue(cacheKey, out WorkoutTemplateResponseDto template))
        {
            _logger.LogInformation("Cache miss. Getting workout template by id {Id} from database", id);
            var dbTemplate = await _workoutTemplateRepository.GetWorkoutTemplateByIdAsync(id);
            if (dbTemplate == null)
            {
                throw new WorkoutTemplateNotFoundException(id);
            }
            template = _mapper.Map<WorkoutTemplateResponseDto>(dbTemplate);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, template, cacheEntryOptions);
        }
        else
        {
            _logger.LogInformation("Cache hit. Returning workout template by id {Id} from cache", id);
        }

        return template;
    }

    public async Task<WorkoutTemplateResponseDto> CreateWorkoutTemplateFromJsonFileAsync(string filePath)
    {
        _logger.LogInformation("Starting to create workout template from JSON file: {FilePath}", filePath);

        string jsonString = await File.ReadAllTextAsync(filePath);
        var jsonDocument = JsonDocument.Parse(jsonString);
        var root = jsonDocument.RootElement.GetProperty("WorkoutTemplate");

        return await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var workoutTemplate = new WorkoutTemplate
                {
                    Name = root.GetProperty("Name").GetString(),
                    Description = root.GetProperty("Description").GetString(),
                    DurationWeeks = root.GetProperty("DurationWeeks").GetInt32()
                };

                await _workoutTemplateRepository.CreateWorkoutTemplateAsync(workoutTemplate);

                var weeks = root.GetProperty("TemplateWeeks").EnumerateArray();
                foreach (var week in weeks)
                {
                    var templateWeek = new TemplateWeek
                    {
                        WorkoutTemplateId = workoutTemplate.Id,
                        WeekNumber = week.GetProperty("WeekNumber").GetInt32()
                    };

                    await _templateWeekRepository.CreateTemplateWeekAsync(templateWeek);

                    var workouts = week.GetProperty("TemplateWorkouts").EnumerateArray();
                    foreach (var workout in workouts)
                    {
                        var templateWorkout = new TemplateWorkout
                        {
                            TemplateWeekId = templateWeek.Id,
                            Name = workout.GetProperty("Name").GetString()
                        };

                        await _templateWorkoutRepository.CreateTemplateWorkoutAsync(templateWorkout);

                        var exercises = workout.GetProperty("TemplateExercises").EnumerateArray();
                        foreach (var exercise in exercises)
                        {
                            var templateExercise = new TemplateExercise
                            {
                                TemplateWorkoutId = templateWorkout.Id,
                                ExerciseName = exercise.GetProperty("ExerciseName").GetString(),
                                LastSetIntensity = exercise.GetProperty("LastSetIntensity").GetString(),
                                WarmupSets = exercise.GetProperty("WarmupSets").GetString(),
                                WorkingSets = exercise.GetProperty("WorkingSets").GetString(),
                                Reps = exercise.GetProperty("Reps").GetString(),
                                Rpe = exercise.GetProperty("Rpe").GetString(),
                                Rest = exercise.GetProperty("Rest").GetString(),
                                Substitution1 = exercise.GetProperty("Substitution1").GetString(),
                                Substitution2 = exercise.GetProperty("Substitution2").GetString(),
                                Notes = exercise.GetProperty("Notes").GetString()
                            };

                            await _templateExerciseRepository.CreateTemplateExerciseAsync(templateExercise);
                        }
                    }
                }
                await transaction.CommitAsync();
                _logger.LogInformation("Successfully created workout template from JSON file");
                return _mapper.Map<WorkoutTemplateResponseDto>(workoutTemplate);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating workout template from JSON file");
                throw;
            }
        });
    }

    public async Task DeleteWorkoutTemplateAsync(Guid templateId)
    {
        _logger.LogInformation("Starting to delete workout template with ID: {TemplateId}", templateId);

        var workoutTemplate = await _workoutTemplateRepository.GetWorkoutTemplateByIdAsync(templateId);
        if (workoutTemplate == null)
        {
            throw new WorkoutTemplateNotFoundException(templateId);
        }

        await _workoutTemplateRepository.DeleteWorkoutTemplateAsync(templateId);

        _logger.LogInformation("Successfully deleted workout template with ID: {TemplateId}", templateId);
    }

    public async Task UpdateTemplateAsync(Guid id, WorkoutTemplateUpdateDto updateDto)
    {
        var existingTemplate = await _workoutTemplateRepository.GetWorkoutTemplateByIdAsync(id);
        if (existingTemplate == null)
        {
            throw new WorkoutTemplateNotFoundException(id);
        }

        _mapper.Map(updateDto, existingTemplate);

        _logger.LogInformation("Updating workout template {TemplateId}", id);

        await _workoutTemplateRepository.UpdateWorkoutTemplateAsync(existingTemplate);

        // Invalidate caches
        _cache.Remove("AllTemplatesBasic");
        _cache.Remove("AllTemplatesDetailed");
        _cache.Remove($"Template_{id}");
        _cache.Remove($"TemplateWithDetails_{id}");
    }
}