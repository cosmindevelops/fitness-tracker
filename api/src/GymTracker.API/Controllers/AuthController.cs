using GymTracker.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">The registration model.</param>
    /// <returns>An IActionResult representing the result of the registration.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModelDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var (Success, UserId, Token) = await _authService.RegisterAsync(model);
        if (!Success)
        {
            return BadRequest("User already exists.");
        }

        return Ok(new { UserId, Token });
    }

    /// <summary>
    /// Logs in a user with the provided credentials.
    /// </summary>
    /// <param name="model">The login model containing user credentials.</param>
    /// <returns>An IActionResult representing the login result.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModelDto model)
    {
        var (Success, UserId, Token) = await _authService.LoginAsync(model);

        if (!Success)
        {
            return BadRequest("Invalid username or password.");
        }

        return Ok(new { UserId, Token });
    }
}