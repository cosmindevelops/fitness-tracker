using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/workouts/{workoutId}/[controller]")]
public class ExerciseController : BaseController
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService, ILogger<ExerciseController> logger) : base(logger)
    {
        _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId)
    {
        Logger.LogInformation("Getting all exercises for workout {WorkoutId} and user {UserId}", workoutId, UserId);
        var results = await _exerciseService.GetAllExercisesForWorkoutAsync(UserId, workoutId);
        return Ok(results);
    }

    [HttpGet("{exerciseId}")]
    public async Task<IActionResult> GetById(Guid workoutId, Guid exerciseId)
    {
        Logger.LogInformation("Getting exercise {ExerciseId} for workout {WorkoutId} and user {UserId}", exerciseId, workoutId, UserId);
        var result = await _exerciseService.GetExerciseByIdAsync(UserId, workoutId, exerciseId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid workoutId, [FromBody] ExerciseCreateDto exerciseDto)
    {
        Logger.LogInformation("Creating exercise for workout {WorkoutId} and user {UserId}", workoutId, UserId);
        var result = await _exerciseService.CreateExerciseAsync(UserId, workoutId, exerciseDto);
        return CreatedAtAction(nameof(GetById), new { workoutId, exerciseId = result.Id }, result);
    }

    [HttpPut("{exerciseId}")]
    public async Task<IActionResult> Update(Guid workoutId, Guid exerciseId, [FromBody] ExerciseUpdateDto exerciseDto)
    {
        Logger.LogInformation("Updating exercise {ExerciseId} for workout {WorkoutId} and user {UserId}", exerciseId, workoutId, UserId);
        await _exerciseService.UpdateExerciseAsync(UserId, workoutId, exerciseId, exerciseDto);
        return NoContent();
    }

    [HttpDelete("{exerciseId}")]
    public async Task<IActionResult> Delete(Guid workoutId, Guid exerciseId)
    {
        Logger.LogInformation("Deleting exercise {ExerciseId} for workout {WorkoutId} and user {UserId}", exerciseId, workoutId, UserId);
        await _exerciseService.DeleteExerciseAsync(UserId, workoutId, exerciseId);
        return NoContent();
    }
}