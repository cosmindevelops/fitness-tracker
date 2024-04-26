using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace GymTracker.API.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult ValidateModel()
    {
        if (!ModelState.IsValid)
        {
            var errorMessages = ModelState.Values.SelectMany(val => val.Errors)
                                                .Select(err => err.ErrorMessage)
                                                .ToList();
            return BadRequest(new { Errors = errorMessages });
        }

        return null;
    }

    protected Guid UserId
    {
        get
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid or missing User ID in the user's claims.");
            }
            return userId;
        }
    }

    protected void LogInformation(string message)
    {
        Log.Information(message);
    }

    protected void LogError(string message)
    {
        Log.Error(message);
    }

    protected void LogError(Exception ex, string message)
    {
        Log.Error(ex, message);
    }
}