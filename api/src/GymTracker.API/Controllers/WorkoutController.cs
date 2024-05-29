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

    public WorkoutController(IWorkoutService workoutService, ILogger<AuthController> logger) : base(logger)
    {
        _workoutService = workoutService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var results = await _workoutService.GetAllWorkoutsForUserAsync(UserId);
        return Ok(results);
    }

    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetById(Guid workoutId)
    {
        var result = await _workoutService.GetWorkoutByIdForUserAsync(UserId, workoutId);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        var results = await _workoutService.GetWorkoutsByNameAsync(UserId, name);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkoutCreateDto workoutDto)
    {
        var result = await _workoutService.CreateWorkoutAsync(UserId, workoutDto);
        return CreatedAtAction(nameof(GetById), new { workoutId = result.Id }, result);
    }

    [HttpPut("{workoutId}")]
    public async Task<IActionResult> Update(Guid workoutId, [FromBody] WorkoutUpdateDto workoutDto)
    {
        await _workoutService.UpdateWorkoutAsync(UserId, workoutId, workoutDto);
        return NoContent();
    }

    [HttpDelete("{workoutId}")]
    public async Task<IActionResult> Delete(Guid workoutId)
    {
        await _workoutService.DeleteWorkoutAsync(UserId, workoutId);
        return NoContent();
    }
}