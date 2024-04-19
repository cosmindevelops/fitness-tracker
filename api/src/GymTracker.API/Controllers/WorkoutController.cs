using AutoMapper;
using GymTracker.Core.DTOs;
using GymTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/[controller]")]
[ApiController]
public class WorkoutController : ControllerBase
{
    private readonly WorkoutService _workoutService;
    private readonly IMapper _mapper;

    public WorkoutController(WorkoutService workoutService, IMapper mapper)
    {
        _workoutService = workoutService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new workout based on the provided workout data.
    /// </summary>
    /// <param name="workoutDto">The workout data to create the workout from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created workout.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkoutCreateDto workoutDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _workoutService.CreateWorkoutAsync(userId, workoutDto);
        return CreatedAtAction(nameof(GetById), new { workoutId = result.Id }, result);
    }

    /// <summary>
    /// Retrieves a workout by its ID for the authenticated user.
    /// </summary>
    /// <param name="workoutId">The ID of the workout to retrieve.</param>
    /// <returns>An IActionResult containing the workout if found, or a 404 Not Found response if not found.</returns>
    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetById(Guid workoutId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _workoutService.GetWorkoutByIdForUserAsync(workoutId, userId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all workouts for the authenticated user.
    /// </summary>
    /// <returns>An IActionResult containing a list of workouts if found, or a 404 Not Found response if not found.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var results = await _workoutService.GetAllWorkoutsForUserAsync(userId);
        return Ok(results);
    }

    /// <summary>
    /// Updates a workout with the specified workoutId for the authenticated user.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout to be updated.</param>
    /// <param name="workoutDto">The WorkoutUpdateDto object containing the updated workout data.</param>
    /// <returns>An IActionResult indicating the result of the update operation.</returns>
    [HttpPut("{workoutId}")]
    public async Task<IActionResult> Update(Guid workoutId, [FromBody] WorkoutUpdateDto workoutDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var success = await _workoutService.UpdateWorkoutAsync(userId, workoutId, workoutDto);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    /// <summary>
    /// Deletes a workout with the specified workoutId for the authenticated user.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout to be deleted.</param>
    /// <returns>An IActionResult indicating the result of the deletion operation. If the deletion is successful, returns NoContent. Otherwise, returns NotFound.</returns>
    [HttpDelete("{workoutId}")]
    public async Task<IActionResult> Delete(Guid workoutId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var success = await _workoutService.DeleteWorkoutAsync(userId, workoutId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}