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
        if (model == null)
        {
            Logger.LogError("Register model is null.");
            return BadRequest("Invalid registration request.");
        }

        var validationResult = ValidateModel(model);
        if (validationResult != null)
        {
            return validationResult;
        }

        await _authService.RegisterAsync(model);

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

        var result = await _authService.LoginAsync(model);

        Logger.LogInformation("User logged in successfully.");
        return Ok(new AuthResponseDto { UserId = result.UserId, Token = result.Token });
    }
}