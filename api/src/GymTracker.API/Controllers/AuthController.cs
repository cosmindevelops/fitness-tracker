using GymTracker.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModelDto model)
    {
        var validationResult = ValidateModel();
        if (validationResult != null)
        {
            return validationResult;
        }

        var (Success, UserId, Token) = await _authService.RegisterAsync(model);
        if (!Success)
        {
            LogError("Attempt to register user failed: User already exists.");
            return BadRequest("User already exists.");
        }

        LogInformation("User registered successfully.");
        return CreatedAtAction(nameof(UserController.GetUserById), "User", new { userId = UserId }, new AuthResponseDto { UserId = Guid.Parse(UserId), Token = Token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModelDto model)
    {
        var validationResult = ValidateModel();
        if (validationResult != null)
        {
            return validationResult;
        }

        var (Success, UserId, Token) = await _authService.LoginAsync(model);
        if (!Success)
        {
            LogError("Login attempt failed: Invalid username or password.");
            return Unauthorized("Invalid username or password.");
        }

        LogInformation("User logged in successfully.");
        return Ok(new AuthResponseDto { UserId = Guid.Parse(UserId), Token = Token });
    }
}