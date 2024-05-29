using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Route("[controller]")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService, ILogger<UserController> logger) : base(logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        Logger.LogInformation("Getting user by id {UserId}", userId);
        var userDto = await _userService.FindByIdAsync(userId);
        if (userDto == null)
        {
            Logger.LogWarning("User not found with id {UserId}", userId);
            return NotFound("User not found.");
        }

        return Ok(userDto);
    }
}