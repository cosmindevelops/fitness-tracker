using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/[controller]")]
[ApiController]
public class UserWorkoutTemplateController : BaseController
{
    private readonly IUserWorkoutTemplateService _userWorkoutTemplateService;

    public UserWorkoutTemplateController(IUserWorkoutTemplateService userWorkoutTemplateService, ILogger<UserWorkoutTemplateController> logger) : base(logger)
    {
        _userWorkoutTemplateService = userWorkoutTemplateService ?? throw new ArgumentNullException(nameof(userWorkoutTemplateService));
    }

    [HttpPost("select")]
    public async Task<IActionResult> SelectTemplateForUser([FromBody] SelectTemplateRequestDto request)
    {
        Logger.LogInformation("User {UserId} selecting template {TemplateId} with start date {StartDate}", UserId, request.TemplateId, request.StartDate);
        var result = await _userWorkoutTemplateService.SelectTemplateForUserAsync(UserId, request.TemplateId, request.StartDate);
        Logger.LogInformation("User {UserId} successfully selected template {TemplateId}", UserId, request.TemplateId);
        return CreatedAtAction(nameof(GetUserWorkoutTemplateById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWorkoutTemplates()
    {
        Logger.LogInformation("Getting workout templates for user {UserId}", UserId);
        var results = await _userWorkoutTemplateService.GetUserWorkoutTemplatesAsync(UserId);
        Logger.LogInformation("Successfully retrieved workout templates for user {UserId}", UserId);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserWorkoutTemplateById(Guid id)
    {
        Logger.LogInformation("Getting workout template {TemplateId} for user {UserId}", id, UserId);
        var result = await _userWorkoutTemplateService.GetUserWorkoutTemplateByIdAsync(UserId, id);
        Logger.LogInformation("Successfully retrieved workout template {TemplateId} for user {UserId}", id, UserId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveTemplateForUser(Guid id)
    {
        Logger.LogInformation("Removing workout template {TemplateId} for user {UserId}", id, UserId);
        await _userWorkoutTemplateService.RemoveTemplateForUserAsync(UserId, id);
        Logger.LogInformation("Successfully removed workout template {TemplateId} for user {UserId}", id, UserId);
        return NoContent();
    }
}