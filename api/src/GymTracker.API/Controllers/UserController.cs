using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Route("[controller]")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService, ILogger<AuthController> logger) : base(logger)
    {
        _userService = userService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var userDto = await _userService.FindByIdAsync(userId);
        if (userDto == null)
        {
            return NotFound("User not found.");
        }

        return Ok(userDto);
    }
}