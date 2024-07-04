using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/[controller]")]
[ApiController]
public class UserExerciseProgressController : BaseController
{
    private readonly IUserExerciseProgressService _userExerciseProgressService;

    public UserExerciseProgressController(IUserExerciseProgressService userExerciseProgressService, ILogger<UserExerciseProgressController> logger) : base(logger)
    {
        _userExerciseProgressService = userExerciseProgressService ?? throw new ArgumentNullException(nameof(userExerciseProgressService));
    }

    [HttpPost("log")]
    public async Task<IActionResult> LogExerciseProgress([FromBody] LogExerciseProgressRequestDto request)
    {
        Logger.LogInformation("Logging exercise progress for user workout template {UserWorkoutTemplateId}, exercise {TemplateExerciseId}", request.UserWorkoutTemplateId, request.TemplateExerciseId);
        var result = await _userExerciseProgressService.LogExerciseProgressAsync(request.UserWorkoutTemplateId, request.TemplateExerciseId, request.ProgressDto);
        return CreatedAtAction(nameof(GetUserExerciseProgressById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExerciseProgress(Guid id, [FromBody] UserExerciseProgressUpdateDto progressDto)
    {
        Logger.LogInformation("Updating exercise progress {ProgressId}", id);
        var result = await _userExerciseProgressService.UpdateExerciseProgressAsync(id, progressDto);
        return Ok(result);
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> MarkWorkoutCompleted(Guid id, [FromBody] bool completed)
    {
        Logger.LogInformation("Marking exercise progress {ProgressId} as {Completed}", id, completed ? "completed" : "not completed");
        var result = await _userExerciseProgressService.MarkWorkoutCompletedAsync(id, completed);
        return Ok(result);
    }

    [HttpPut("{id}/reset")]
    public async Task<IActionResult> ResetExerciseProgress(Guid id)
    {
        Logger.LogInformation("Resetting exercise progress {ProgressId}", id);
        await _userExerciseProgressService.ResetExerciseProgressAsync(id);
        return NoContent();
    }

    [HttpGet("template/{userWorkoutTemplateId}")]
    public async Task<IActionResult> GetUserExerciseProgressByUserWorkoutTemplateId(Guid userWorkoutTemplateId)
    {
        Logger.LogInformation("Getting exercise progress for user workout template {UserWorkoutTemplateId}", userWorkoutTemplateId);
        var results = await _userExerciseProgressService.GetUserExerciseProgressByUserWorkoutTemplateIdAsync(userWorkoutTemplateId);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserExerciseProgressById(Guid id)
    {
        Logger.LogInformation("Getting exercise progress {ProgressId}", id);
        var result = await _userExerciseProgressService.GetUserExerciseProgressByIdAsync(id);
        return Ok(result);
    }
}