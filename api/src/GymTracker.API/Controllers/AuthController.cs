using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration) : base(logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [AllowAnonymous]
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse));
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var (user, token) = await _authService.HandleGoogleResponse(HttpContext);
        if (user != null)
        {
            var frontendBaseUrl = _configuration["RedirectUrls:FrontendBaseUrl"];
            var redirectUrl = $"{frontendBaseUrl}/home?token={System.Net.WebUtility.UrlEncode(token)}";
            return Redirect(redirectUrl);
        }
        return BadRequest("Google authentication failed.");
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
        return Ok(new { Message = "User registered successfully." });
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