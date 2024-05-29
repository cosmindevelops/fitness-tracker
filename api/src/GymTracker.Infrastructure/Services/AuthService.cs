using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GymTracker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IPasswordHasher<User> passwordHasher, ILogger<AuthService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RegisterAsync(RegisterModelDto model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        _logger.LogInformation("Registering user {Username}", model.Username);

        if (await _userRepository.UserExistsAsync(model.Username, model.Email))
        {
            _logger.LogWarning("User already exists with username {Username} or email {Email}", model.Username, model.Email);
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

        _logger.LogInformation("User {Username} registered successfully", model.Username);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginModelDto model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        _logger.LogInformation("Logging in user with email {Email}", model.Email);

        var user = await _userRepository.FindByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogWarning("Invalid login attempt: email {Email} not found", model.Email);
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verificationResult != PasswordVerificationResult.Success)
        {
            _logger.LogWarning("Invalid login attempt for email {Email}: incorrect password", model.Email);
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var roles = await _userRepository.GetUserRolesAsync(user);

        var token = _jwtTokenService.GenerateToken(user, roles);

        _logger.LogInformation("User {Username} logged in successfully", user.Username);

        return new AuthResponseDto { UserId = user.Id, Token = token };
    }
}