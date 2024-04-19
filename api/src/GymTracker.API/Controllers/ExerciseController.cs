using AutoMapper;
using GymTracker.Core.DTOs;
using GymTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/workouts/{workoutId}/[controller]")]
[ApiController]
public class ExerciseController : ControllerBase
{
    private readonly ExerciseService _exerciseService;
    private readonly IMapper _mapper;

    public ExerciseController(ExerciseService exerciseService, IMapper mapper)
    {
        _exerciseService = exerciseService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new exercise for a specific workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout for which the exercise is created.</param>
    /// <param name="exerciseDto">The ExerciseCreateDto object containing the exercise data.</param>
    /// <returns>An IActionResult representing the outcome of the exercise creation process.</returns>
    public async Task<IActionResult> Create(Guid workoutId, [FromBody] ExerciseCreateDto exerciseDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _exerciseService.CreateExerciseAsync(userId, workoutId, exerciseDto);

        if (result == null)
        {
            return BadRequest($"Workout with ID {workoutId} does not exist or does not belong to the user.");
        }

        return CreatedAtAction(nameof(GetById), new { workoutId, exerciseId = result.Id }, result);
    }

    /// <summary>
    /// Retrieves a specific exercise by ID for a given workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout.</param>
    /// <param name="exerciseId">The unique identifier of the exercise.</param>
    /// <returns>An IActionResult containing the specific exercise information.</returns>
    [HttpGet("{exerciseId}")]
    public async Task<IActionResult> GetById(Guid workoutId, Guid exerciseId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _exerciseService.GetExerciseByIdAsync(userId, workoutId, exerciseId);

        if (result == null)
        {
            return NotFound("Exercise not found or does not belong to the specified workout.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all exercises for a given workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout.</param>
    /// <returns>An IActionResult containing the results of the operation.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var results = await _exerciseService.GetAllExercisesForWorkoutAsync(userId, workoutId);
        return Ok(results);
    }

    /// <summary>
    /// Updates an exercise for a specific workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout.</param>
    /// <param name="exerciseId">The unique identifier of the exercise.</param>
    /// <param name="exerciseDto">The ExerciseUpdateDto object containing the updated exercise data.</param>
    /// <returns>An IActionResult indicating the result of the update operation.</returns>
    [HttpPut("{exerciseId}")]
    public async Task<IActionResult> Update(Guid workoutId, Guid exerciseId, [FromBody] ExerciseUpdateDto exerciseDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var success = await _exerciseService.UpdateExerciseAsync(userId, workoutId, exerciseId, exerciseDto);

        if (!success)
        {
            return NotFound("Exercise not found or does not belong to the specified workout.");
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes an exercise by its ID from a specific workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout.</param>
    /// <param name="exerciseId">The unique identifier of the exercise.</param>
    /// <returns>An IActionResult indicating the result of the deletion operation.</returns>
    [HttpDelete("{exerciseId}")]
    public async Task<IActionResult> Delete(Guid workoutId, Guid exerciseId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var success = await _exerciseService.DeleteExerciseAsync(userId, workoutId, exerciseId);

        if (!success)
        {
            return NotFound("Exercise not found or does not belong to the specified workout.");
        }

        return NoContent();
    }
}