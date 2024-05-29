using GymTracker.Infrastructure.Common;
using GymTracker.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/workouts/{workoutId}/exercises/{exerciseId}/[controller]")]
public class SeriesController : BaseController
{
    private readonly ISeriesService _seriesService;

    public SeriesController(ISeriesService seriesService, ILogger<SeriesController> logger) : base(logger)
    {
        _seriesService = seriesService ?? throw new ArgumentNullException(nameof(seriesService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId, Guid exerciseId)
    {
        Logger.LogInformation("Getting all series for exercise {ExerciseId}, workout {WorkoutId}, and user {UserId}", exerciseId, workoutId, UserId);
        var results = await _seriesService.GetAllSeriesForExerciseAsync(UserId, workoutId, exerciseId);
        return Ok(results);
    }

    [HttpGet("{seriesId}")]
    public async Task<IActionResult> GetById(Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        Logger.LogInformation("Getting series {SeriesId} for exercise {ExerciseId}, workout {WorkoutId}, and user {UserId}", seriesId, exerciseId, workoutId, UserId);
        var result = await _seriesService.GetSeriesByIdAsync(UserId, workoutId, exerciseId, seriesId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid workoutId, Guid exerciseId, [FromBody] SeriesCreateDto seriesDto)
    {
        Logger.LogInformation("Creating series for exercise {ExerciseId}, workout {WorkoutId}, and user {UserId}", exerciseId, workoutId, UserId);
        var result = await _seriesService.CreateSeriesAsync(UserId, workoutId, exerciseId, seriesDto);
        return CreatedAtAction(nameof(GetById), new { workoutId, exerciseId, seriesId = result.Id }, result);
    }

    [HttpPut("{seriesId}")]
    public async Task<IActionResult> Update(Guid workoutId, Guid exerciseId, Guid seriesId, [FromBody] SeriesUpdateDto seriesDto)
    {
        Logger.LogInformation("Updating series {SeriesId} for exercise {ExerciseId}, workout {WorkoutId}, and user {UserId}", seriesId, exerciseId, workoutId, UserId);
        await _seriesService.UpdateSeriesAsync(UserId, workoutId, exerciseId, seriesId, seriesDto);
        return NoContent();
    }

    [HttpDelete("{seriesId}")]
    public async Task<IActionResult> Delete(Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        Logger.LogInformation("Deleting series {SeriesId} for exercise {ExerciseId}, workout {WorkoutId}, and user {UserId}", seriesId, exerciseId, workoutId, UserId);
        await _seriesService.DeleteSeriesAsync(UserId, workoutId, exerciseId, seriesId);
        return NoContent();
    }
}