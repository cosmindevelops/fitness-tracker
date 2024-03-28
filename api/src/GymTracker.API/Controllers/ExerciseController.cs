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

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var results = await _exerciseService.GetAllExercisesForWorkoutAsync(userId, workoutId);
        return Ok(results);
    }

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