using AutoMapper;
using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/workouts/{workoutId}/[controller]")]
public class ExerciseController : BaseController
{
    private readonly ExerciseService _exerciseService;

    public ExerciseController(ExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId)
    {
        var results = await _exerciseService.GetAllExercisesForWorkoutAsync(UserId, workoutId);
        return Ok(results);
    }

    [HttpGet("{exerciseId}")]
    public async Task<IActionResult> GetById(Guid workoutId, Guid exerciseId)
    {
        var result = await _exerciseService.GetExerciseByIdAsync(UserId, workoutId, exerciseId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid workoutId, [FromBody] ExerciseCreateDto exerciseDto)
    {
        var result = await _exerciseService.CreateExerciseAsync(UserId, workoutId, exerciseDto);
        return CreatedAtAction(nameof(GetById), new { workoutId, exerciseId = result.Id }, result);
    }

    [HttpPut("{exerciseId}")]
    public async Task<IActionResult> Update(Guid workoutId, Guid exerciseId, [FromBody] ExerciseUpdateDto exerciseDto)
    {
        await _exerciseService.UpdateExerciseAsync(UserId, workoutId, exerciseId, exerciseDto);
        return NoContent();
    }

    [HttpDelete("{exerciseId}")]
    public async Task<IActionResult> Delete(Guid workoutId, Guid exerciseId)
    {
        await _exerciseService.DeleteExerciseAsync(UserId, workoutId, exerciseId);
        return NoContent();
    }
}