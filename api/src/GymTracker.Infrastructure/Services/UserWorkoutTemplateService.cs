using AutoMapper;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Services;

public class UserWorkoutTemplateService : IUserWorkoutTemplateService
{
    private readonly IUserWorkoutTemplateRepository _userWorkoutTemplateRepository;
    private readonly IWorkoutTemplateRepository _workoutTemplateRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserWorkoutTemplateService> _logger;
    private readonly IMapper _mapper;

    public UserWorkoutTemplateService(
        IUserWorkoutTemplateRepository userWorkoutTemplateRepository,
        IWorkoutTemplateRepository workoutTemplateRepository,
        IUserRepository userRepository,
        ILogger<UserWorkoutTemplateService> logger,
        IMapper mapper)
    {
        _userWorkoutTemplateRepository = userWorkoutTemplateRepository ?? throw new ArgumentNullException(nameof(userWorkoutTemplateRepository));
        _workoutTemplateRepository = workoutTemplateRepository ?? throw new ArgumentNullException(nameof(workoutTemplateRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<UserWorkoutTemplateResponseDto> SelectTemplateForUserAsync(Guid userId, Guid templateId, DateTime startDate)
    {
        // Check if the user exists
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        // Check if the workout template exists
        var template = await _workoutTemplateRepository.GetWorkoutTemplateByIdAsync(templateId);
        if (template == null)
        {
            throw new WorkoutTemplateNotFoundException(templateId);
        }

        // Check if the user has already selected this template
        var existingUserWorkoutTemplate = await _userWorkoutTemplateRepository.GetUserWorkoutTemplateByUserIdAndTemplateIdAsync(userId, templateId);
        if (existingUserWorkoutTemplate != null)
        {
            throw new TemplateAlreadySelectedException(userId, templateId);
        }

        // Create a new UserWorkoutTemplate
        var userWorkoutTemplate = new UserWorkoutTemplate
        {
            UserId = userId,
            WorkoutTemplateId = templateId,
            StartDate = startDate
        };

        var result = await _userWorkoutTemplateRepository.CreateUserWorkoutTemplateAsync(userWorkoutTemplate);
        _logger.LogInformation($"User {userId} selected workout template {templateId}");
        return _mapper.Map<UserWorkoutTemplateResponseDto>(result);
    }

    public async Task<IEnumerable<UserWorkoutTemplateResponseDto>> GetUserWorkoutTemplatesAsync(Guid userId)
    {
        // Check if the user exists
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        // Get all workout templates for the user
        var userWorkoutTemplates = await _userWorkoutTemplateRepository.GetUserWorkoutTemplatesByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<UserWorkoutTemplateResponseDto>>(userWorkoutTemplates);
    }

    public async Task<UserWorkoutTemplateResponseDto> GetUserWorkoutTemplateByIdAsync(Guid userId, Guid userWorkoutTemplateId)
    {
        // Get the specific UserWorkoutTemplate
        var userWorkoutTemplate = await _userWorkoutTemplateRepository.GetUserWorkoutTemplateByIdAsync(userWorkoutTemplateId);
        if (userWorkoutTemplate == null || userWorkoutTemplate.UserId != userId)
        {
            throw new UserWorkoutTemplateNotFoundException(userWorkoutTemplateId, userId);
        }

        return _mapper.Map<UserWorkoutTemplateResponseDto>(userWorkoutTemplate);
    }

    public async Task RemoveTemplateForUserAsync(Guid userId, Guid userWorkoutTemplateId)
    {
        // Check if the UserWorkoutTemplate exists and belongs to the user
        var userWorkoutTemplate = await _userWorkoutTemplateRepository.GetUserWorkoutTemplateByIdAsync(userWorkoutTemplateId);
        if (userWorkoutTemplate == null || userWorkoutTemplate.UserId != userId)
        {
            throw new UserWorkoutTemplateNotFoundException(userWorkoutTemplateId, userId);
        }

        // Remove the UserWorkoutTemplate
        await _userWorkoutTemplateRepository.DeleteUserWorkoutTemplateAsync(userWorkoutTemplateId);
        _logger.LogInformation($"Removed workout template {userWorkoutTemplate.WorkoutTemplateId} for user {userId}");
    }
}