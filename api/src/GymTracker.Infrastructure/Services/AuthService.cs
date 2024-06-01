using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

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

    public async Task<(User, string)> HandleGoogleResponse(HttpContext httpContext)
    {
        var result = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Google authentication failed.");
            return (null, null);
        }

        var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        {
            claim.Issuer,
            claim.OriginalIssuer,
            claim.Type,
            claim.Value
        });

        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email != null)
        {
            var user = await FindOrCreateGoogleUserAsync(email);
            var roles = await GetUserRolesAsync(user);
            var token = _jwtTokenService.GenerateToken(user, roles);
            return (user, token);
        }
        _logger.LogWarning("Email claim not found.");
        return (null, null);
    }

    public async Task<User> FindOrCreateGoogleUserAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentNullException(nameof(email));
        }

        var user = await _userRepository.FindByEmailAsync(email);
        if (user == null)
        {
            var username = email.Split('@')[0];
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = _passwordHasher.HashPassword(null, Guid.NewGuid().ToString()) // Dummy password
            };
            await _userRepository.RegisterUserAsync(user, "User");
        }
        return user;
    }

    public async Task<List<string>> GetUserRolesAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        return await _userRepository.GetUserRolesAsync(user);
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