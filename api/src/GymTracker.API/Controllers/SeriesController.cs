using GymTracker.Core.DTOs;
using GymTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymTracker.API.Controllers;

[Authorize(Roles = "Admin,User,Moderator")]
[Route("api/workouts/{workoutId}/exercises/{exerciseId}/[controller]")]
[ApiController]
public class SeriesController : ControllerBase
{
    private readonly SeriesService _seriesService;

    public SeriesController(SeriesService seriesService)
    {
        _seriesService = seriesService;
    }

    /// <summary>
    /// Creates a new series for a specific exercise within a workout based on the provided workout ID, exercise ID, and series data.
    /// </summary>
    /// <param name="workoutId">The ID of the workout.</param>
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <param name="seriesDto">The series data to create the series.</param>
    /// <returns>An IActionResult containing the created series.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(Guid workoutId, Guid exerciseId, [FromBody] SeriesCreateDto seriesDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _seriesService.CreateSeriesAsync(userId, workoutId, exerciseId, seriesDto);
        return CreatedAtAction(nameof(GetById), new { workoutId, exerciseId, seriesId = result.Id }, result);
    }


    /// <summary>
    /// Retrieves a series by its ID, along with the IDs of the workout and exercise it belongs to.
    /// </summary>
    /// <param name="workoutId">The ID of the workout.</param>
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <param name="seriesId">The ID of the series.</param>
    /// <returns>An IActionResult containing the series if found, or a 404 Not Found response if not found.</returns>
    [HttpGet("{seriesId}")]
    public async Task<IActionResult> GetById(Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _seriesService.GetSeriesByIdAsync(userId, workoutId, exerciseId, seriesId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all series for a specific exercise within a workout.
    /// </summary>
    /// <param name="workoutId">The ID of the workout.</param>
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <returns>An IActionResult containing the series for the specified exercise within the workout.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId, Guid exerciseId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var results = await _seriesService.GetAllSeriesForExerciseAsync(userId, workoutId, exerciseId);
        return Ok(results);
    }

    /// <summary>
    /// Updates a series by its ID.
    /// </summary>
    /// <param name="workoutId">The ID of the workout.</param>
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <param name="seriesId">The ID of the series.</param>
    /// <param name="seriesDto">The updated series data.</param>
    /// <returns>An IActionResult indicating the success or failure of the update operation.</returns>
    [HttpPut("{seriesId}")]
    public async Task<IActionResult> Update(Guid workoutId, Guid exerciseId, Guid seriesId, [FromBody] SeriesUpdateDto seriesDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var success = await _seriesService.UpdateSeriesAsync(userId, workoutId, exerciseId, seriesId, seriesDto);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a series by its ID.
    /// </summary>
    /// <param name="workoutId">The ID of the workout.</param>
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <param name="seriesId">The ID of the series.</param>
    /// <returns>An IActionResult indicating the success or failure of the deletion operation.</returns>
    [HttpDelete("{seriesId}")]
    public async Task<IActionResult> Delete(Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var success = await _seriesService.DeleteSeriesAsync(userId, workoutId, exerciseId, seriesId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}