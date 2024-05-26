using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymTracker.API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected readonly ILogger Logger;

    protected BaseController(ILogger logger)
    {
        Logger = logger;
    }

    protected IActionResult ValidateModel(object model)
    {
        if (!TryValidateModel(model))
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
                Logger.LogError("Invalid or missing User ID in the user's claims.");
                throw new UnauthorizedAccessException("Invalid or missing User ID in the user's claims.");
            }
            return userId;
        }
    }
}