using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GymTracker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task RegisterAsync(RegisterModelDto model)
    {
        if (await _userRepository.UserExistsAsync(model.Username, model.Email))
        {
            throw new UserAlreadyExistsException("User already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = model.Username,
            Email = model.Email,
            PasswordHash = _passwordHasher.HashPassword(null, model.Password)
        };

        await _userRepository.RegisterUserAsync(user, "User");
    }

    public async Task<AuthResponseDto> LoginAsync(LoginModelDto model)
    {
        var user = await _userRepository.FindByEmailAsync(model.Email);
        if (user == null)
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verificationResult != PasswordVerificationResult.Success)
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var roles = await _userRepository.GetUserRolesAsync(user);

        var token = _jwtTokenService.GenerateToken(user, roles);
        return new AuthResponseDto { UserId = user.Id, Token = token };
    }
}