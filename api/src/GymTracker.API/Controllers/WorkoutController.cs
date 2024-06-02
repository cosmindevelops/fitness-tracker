using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/[controller]")]
public class WorkoutController : BaseController
{
    private readonly IWorkoutService _workoutService;

    public WorkoutController(IWorkoutService workoutService, ILogger<WorkoutController> logger) : base(logger)
    {
        _workoutService = workoutService ?? throw new ArgumentNullException(nameof(workoutService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Logger.LogInformation("Getting all workouts for user {UserId}", UserId);
        var results = await _workoutService.GetAllWorkoutsForUserAsync(UserId);
        return Ok(results);
    }

    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetById(Guid workoutId)
    {
        Logger.LogInformation("Getting workout {WorkoutId} for user {UserId}", workoutId, UserId);
        var result = await _workoutService.GetWorkoutByIdForUserAsync(UserId, workoutId);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchWorkouts([FromQuery] string name, [FromQuery] DateTime? date)
    {
        Logger.LogInformation("Searching workouts by name {Name} and date {Date} for user {UserId}", name, date, UserId);
        var results = await _workoutService.SearchWorkoutsAsync(UserId, name, date);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkoutCreateDto workoutDto)
    {
        Logger.LogInformation("Creating workout for user {UserId}", UserId);

        workoutDto.Date = DateTime.SpecifyKind(workoutDto.Date, DateTimeKind.Utc);

        var result = await _workoutService.CreateWorkoutAsync(UserId, workoutDto);
        return CreatedAtAction(nameof(GetById), new { workoutId = result.Id }, result);
    }

    [HttpPut("{workoutId}")]
    public async Task<IActionResult> Update(Guid workoutId, [FromBody] WorkoutUpdateDto workoutDto)
    {
        Logger.LogInformation("Updating workout {WorkoutId} for user {UserId}", workoutId, UserId);
        await _workoutService.UpdateWorkoutAsync(UserId, workoutId, workoutDto);
        return NoContent();
    }

    [HttpDelete("{workoutId}")]
    public async Task<IActionResult> Delete(Guid workoutId)
    {
        Logger.LogInformation("Deleting workout {WorkoutId} for user {UserId}", workoutId, UserId);
        await _workoutService.DeleteWorkoutAsync(UserId, workoutId);
        return NoContent();
    }
}