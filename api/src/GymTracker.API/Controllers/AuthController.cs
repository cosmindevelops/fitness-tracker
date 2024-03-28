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