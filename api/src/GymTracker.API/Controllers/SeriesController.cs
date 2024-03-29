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

    [HttpPost]
    public async Task<IActionResult> Create(Guid workoutId, Guid exerciseId, [FromBody] SeriesCreateDto seriesDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _seriesService.CreateSeriesAsync(userId, workoutId, exerciseId, seriesDto);
        return CreatedAtAction(nameof(GetById), new { workoutId, exerciseId, seriesId = result.Id }, result);
    }

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

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid workoutId, Guid exerciseId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var results = await _seriesService.GetAllSeriesForExerciseAsync(userId, workoutId, exerciseId);
        return Ok(results);
    }

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