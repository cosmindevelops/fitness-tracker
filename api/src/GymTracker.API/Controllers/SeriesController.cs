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

    public SeriesController(ISeriesService seriesService, ILogger<AuthController> logger) : base(logger)
    {
        _seriesService = seriesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId, Guid exerciseId)
    {
        var results = await _seriesService.GetAllSeriesForExerciseAsync(UserId, workoutId, exerciseId);
        return Ok(results);
    }

    [HttpGet("{seriesId}")]
    public async Task<IActionResult> GetById(Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        var result = await _seriesService.GetSeriesByIdAsync(UserId, workoutId, exerciseId, seriesId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid workoutId, Guid exerciseId, [FromBody] SeriesCreateDto seriesDto)
    {
        var result = await _seriesService.CreateSeriesAsync(UserId, workoutId, exerciseId, seriesDto);
        return CreatedAtAction(nameof(GetById), new { workoutId, exerciseId, seriesId = result.Id }, result);
    }

    [HttpPut("{seriesId}")]
    public async Task<IActionResult> Update(Guid workoutId, Guid exerciseId, Guid seriesId, [FromBody] SeriesUpdateDto seriesDto)
    {
        await _seriesService.UpdateSeriesAsync(UserId, workoutId, exerciseId, seriesId, seriesDto);
        return NoContent();
    }

    [HttpDelete("{seriesId}")]
    public async Task<IActionResult> Delete(Guid workoutId, Guid exerciseId, Guid seriesId)
    {
        await _seriesService.DeleteSeriesAsync(UserId, workoutId, exerciseId, seriesId);
        return NoContent();
    }
}