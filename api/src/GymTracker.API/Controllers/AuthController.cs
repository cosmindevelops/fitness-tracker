using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, ILogger<AuthController> logger) : base(logger)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDto model)
    {
        var validationResult = ValidateModel(model);
        if (validationResult != null)
        {
            return validationResult;
        }

        var (Success, UserId, Token) = await _authService.RegisterAsync(model);
        if (!Success)
        {
            Logger.LogError("Attempt to register user failed: User already exists.");
            return BadRequest("User already exists.");
        }

        Logger.LogInformation("User registered successfully.");
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDto model)
    {
        if (model == null)
        {
            Logger.LogError("Login model is null.");
            return BadRequest("Invalid login request.");
        }

        var validationResult = ValidateModel(model);
        if (validationResult != null)
        {
            return validationResult;
        }

        var (Success, UserId, Token) = await _authService.LoginAsync(model);
        if (!Success)
        {
            Logger.LogError("Login attempt failed: Invalid username or password.");
            return Unauthorized("Invalid username or password.");
        }

        Logger.LogInformation("User logged in successfully.");
        return Ok(new AuthResponseDto { UserId = Guid.Parse(UserId), Token = Token });
    }
}