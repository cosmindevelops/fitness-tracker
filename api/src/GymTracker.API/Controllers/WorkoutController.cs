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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkoutCreateDto workoutDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _workoutService.CreateWorkoutAsync(userId, workoutDto);
        return CreatedAtAction(nameof(GetById), new { workoutId = result.Id }, result);
    }

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var results = await _workoutService.GetAllWorkoutsForUserAsync(userId);
        return Ok(results);
    }

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