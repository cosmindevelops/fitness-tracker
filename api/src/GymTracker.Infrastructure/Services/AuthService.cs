using GymTracker.Core.Common;
using GymTracker.Core.Entities;
using GymTracker.Infrastructure.Authentication;
using GymTracker.Infrastructure.Repositories.Interfaces;
using GymTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, JwtTokenService jwtTokenService, PasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<(bool Success, string UserId, string Token)> RegisterAsync(RegisterModelDto model)
    {
        var userExists = await _userRepository.UserExistsAsync(model.Username, model.Email);
        if (userExists)
        {
            return (false, null, null);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = model.Username,
            Email = model.Email,
            PasswordHash = _passwordHasher.HashPassword(null, model.Password)
        };

        user = await _userRepository.RegisterUserAsync(user, "User");

        var roles = await _userRepository.GetUserRolesAsync(user);

        var token = _jwtTokenService.GenerateToken(user, roles);

        return (true, user.Id.ToString(), token);
    }

    public async Task<(bool Success, string UserId, string Token)> LoginAsync(LoginModelDto model)
    {
        var user = await _userRepository.FindByUsernameAsync(model.Username);

        if (user == null) return (false, null, null);

        var verificationResult = _passwordHasher.VerifyPassword(user, model.Password, user.PasswordHash);
        if (verificationResult != PasswordVerificationResult.Success) return (false, null, null);

        var roles = await _userRepository.GetUserRolesAsync(user);

        var token = _jwtTokenService.GenerateToken(user, roles);
        return (true, user.Id.ToString(), token);
    }
}
