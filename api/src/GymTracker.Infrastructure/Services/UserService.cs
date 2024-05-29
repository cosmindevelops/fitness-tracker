using AutoMapper;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility.Interfaces;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IGuidValidator _guidValidator;
    private readonly IEntityValidator _entityValidator;

    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger, IGuidValidator guidValidator, IEntityValidator entityValidator)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _guidValidator = guidValidator ?? throw new ArgumentNullException(nameof(guidValidator));
        _entityValidator = entityValidator ?? throw new ArgumentNullException(nameof(entityValidator));
    }

    public async Task<UserDto> FindByIdAsync(Guid userId)
    {
        _guidValidator.Validate(userId);

        _logger.LogInformation("Finding user by id {UserId}", userId);

        var user = await _userRepository.FindByIdAsync(userId);
        _entityValidator.EnsureUserExists(user, userId);

        return _mapper.Map<UserDto>(user);
    }
}