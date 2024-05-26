using AutoMapper;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Utility;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;

namespace GymTracker.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> FindByIdAsync(Guid userId)
    {
        GuidValidator.Validate(userId);

        var user = await _userRepository.FindByIdAsync(userId);
        EntityValidator.EnsureUserExists(user, userId);

        return _mapper.Map<UserDto>(user);
    }
}