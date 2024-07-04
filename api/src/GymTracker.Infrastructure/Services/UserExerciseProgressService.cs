using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Services;

public class UserExerciseProgressService : IUserExerciseProgressService
{
    private readonly IUserExerciseProgressRepository _userExerciseProgressRepository;
    private readonly IUserWorkoutTemplateRepository _userWorkoutTemplateRepository;
    private readonly ILogger<UserExerciseProgressService> _logger;
    private readonly IMapper _mapper;

    public UserExerciseProgressService(
        IUserExerciseProgressRepository userExerciseProgressRepository,
        IUserWorkoutTemplateRepository userWorkoutTemplateRepository,
        ILogger<UserExerciseProgressService> logger,
        IMapper mapper)
    {
        _userExerciseProgressRepository = userExerciseProgressRepository ?? throw new ArgumentNullException(nameof(userExerciseProgressRepository));
        _userWorkoutTemplateRepository = userWorkoutTemplateRepository ?? throw new ArgumentNullException(nameof(userWorkoutTemplateRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<UserExerciseProgressResponseDto> LogExerciseProgressAsync(Guid userWorkoutTemplateId, Guid templateExerciseId, UserExerciseProgressUpdateDto progressDto)
    {
        // Check if the user workout template exists
        var userWorkoutTemplate = await _userWorkoutTemplateRepository.GetUserWorkoutTemplateByIdAsync(userWorkoutTemplateId);
        if (userWorkoutTemplate == null)
        {
            throw new UserWorkoutTemplateNotFoundException(userWorkoutTemplateId);
        }

        // Get existing progress for this user workout template
        var existingProgress = await _userExerciseProgressRepository.GetUserExerciseProgressByUserWorkoutTemplateIdAsync(userWorkoutTemplateId);
        var progress = existingProgress.FirstOrDefault(p => p.TemplateExerciseId == templateExerciseId);

        if (progress == null)
        {
            // If no progress exists, create a new one
            progress = new UserExerciseProgress
            {
                UserWorkoutTemplateId = userWorkoutTemplateId,
                TemplateExerciseId = templateExerciseId
            };
            _mapper.Map(progressDto, progress);
            progress = await _userExerciseProgressRepository.CreateUserExerciseProgressAsync(progress);
        }
        else
        {
            // If progress exists, update it
            _mapper.Map(progressDto, progress);
            await _userExerciseProgressRepository.UpdateUserExerciseProgressAsync(progress);
        }

        _logger.LogInformation($"Logged exercise progress for user workout template {userWorkoutTemplateId}, exercise {templateExerciseId}");
        return _mapper.Map<UserExerciseProgressResponseDto>(progress);
    }

    public async Task<UserExerciseProgressResponseDto> UpdateExerciseProgressAsync(Guid progressId, UserExerciseProgressUpdateDto progressDto)
    {
        // Find the existing progress
        var progress = await _userExerciseProgressRepository.GetUserExerciseProgressByIdAsync(progressId);
        if (progress == null)
        {
            throw new UserExerciseProgressNotFoundException(progressId);
        }

        // Update the progress with new data
        _mapper.Map(progressDto, progress);
        await _userExerciseProgressRepository.UpdateUserExerciseProgressAsync(progress);

        _logger.LogInformation($"Updated exercise progress {progressId}");
        return _mapper.Map<UserExerciseProgressResponseDto>(progress);
    }

    public async Task<UserExerciseProgressResponseDto> MarkWorkoutCompletedAsync(Guid progressId, bool completed)
    {
        // Find the existing progress
        var progress = await _userExerciseProgressRepository.GetUserExerciseProgressByIdAsync(progressId);
        if (progress == null)
        {
            throw new UserExerciseProgressNotFoundException(progressId);
        }

        // Update completion status and date
        progress.WorkoutCompleted = completed;
        progress.CompletionDate = completed ? DateTime.UtcNow : null;

        await _userExerciseProgressRepository.UpdateUserExerciseProgressAsync(progress);

        _logger.LogInformation($"Marked exercise progress {progressId} as {(completed ? "completed" : "incomplete")}");
        return _mapper.Map<UserExerciseProgressResponseDto>(progress);
    }

    public async Task ResetExerciseProgressAsync(Guid progressId)
    {
        // Find the existing progress
        var progress = await _userExerciseProgressRepository.GetUserExerciseProgressByIdAsync(progressId);
        if (progress == null)
        {
            throw new UserExerciseProgressNotFoundException(progressId);
        }

        // Reset all progress fields
        progress.Set1Reps = null;
        progress.Set2Reps = null;
        progress.Set3Reps = null;
        progress.Set4Reps = null;
        progress.WorkoutCompleted = false;
        progress.CompletionDate = null;

        await _userExerciseProgressRepository.UpdateUserExerciseProgressAsync(progress);

        _logger.LogInformation($"Reset exercise progress {progressId}");
    }

    public async Task<IEnumerable<UserExerciseProgressResponseDto>> GetUserExerciseProgressByUserWorkoutTemplateIdAsync(Guid userWorkoutTemplateId)
    {
        // Retrieves all exercise progress entries for a given user workout template.
        var progress = await _userExerciseProgressRepository.GetUserExerciseProgressByUserWorkoutTemplateIdAsync(userWorkoutTemplateId);
        return _mapper.Map<IEnumerable<UserExerciseProgressResponseDto>>(progress);
    }

    public async Task<UserExerciseProgressResponseDto> GetUserExerciseProgressByIdAsync(Guid progressId)
    {
        var progress = await _userExerciseProgressRepository.GetUserExerciseProgressByIdAsync(progressId);
        if (progress == null)
        {
            throw new UserExerciseProgressNotFoundException(progressId);
        }

        return _mapper.Map<UserExerciseProgressResponseDto>(progress);
    }
}