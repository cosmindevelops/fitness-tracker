using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/[controller]")]
public class WorkoutTemplateController : BaseController
{
    private readonly IWorkoutTemplateService _workoutTemplateService;

    public WorkoutTemplateController(IWorkoutTemplateService workoutTemplateService, ILogger<WorkoutTemplateController> logger) : base(logger)
    {
        _workoutTemplateService = workoutTemplateService ?? throw new ArgumentNullException(nameof(workoutTemplateService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDetails = false)
    {
        Logger.LogInformation("Getting all workout templates. Include details: {IncludeDetails}", includeDetails);
        var results = await _workoutTemplateService.GetAllTemplatesAsync(includeDetails);
        Logger.LogInformation("Returned {Count} templates", results.Count());
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeDetails = false)
    {
        Logger.LogInformation("Getting workout template {TemplateId}. Include details: {IncludeDetails}", id, includeDetails);
        var result = includeDetails
            ? await _workoutTemplateService.GetTemplateByIdWithDetailsAsync(id)
            : await _workoutTemplateService.GetTemplateByIdAsync(id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] WorkoutTemplateUpdateDto updateDto)
    {
        Logger.LogInformation("Updating workout template {TemplateId}", id);
        await _workoutTemplateService.UpdateTemplateAsync(id, updateDto);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Logger.LogInformation("Deleting workout template {TemplateId}", id);
        await _workoutTemplateService.DeleteWorkoutTemplateAsync(id);
        return NoContent();
    }
}