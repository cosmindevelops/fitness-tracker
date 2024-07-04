using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Exceptions;
using System.Net;
using System.Text.Json;

namespace GymTracker.API.Exceptions;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred processing the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case WorkoutNotFoundException _:
            case UserNotFoundException _:
            case ExerciseNotFoundException _:
            case SeriesNotFoundException _:
            case WorkoutTemplateNotFoundException _:
            case UserWorkoutTemplateNotFoundException _:
            case UserExerciseProgressNotFoundException _:
                code = HttpStatusCode.NotFound; // 404
                result = JsonSerializer.Serialize(new { error = exception.Message });
                break;

            case UnauthorizedAccessException _:
                code = HttpStatusCode.Unauthorized; // 401
                result = JsonSerializer.Serialize(new { error = exception.Message });
                break;

            case TemplateAlreadySelectedException _:
            case UserAlreadyExistsException _:
                code = HttpStatusCode.Conflict; // 409
                result = JsonSerializer.Serialize(new { error = exception.Message });
                break;

            case InvalidCredentialsException _:
                code = HttpStatusCode.Unauthorized; // 401
                result = JsonSerializer.Serialize(new { error = exception.Message });
                break;

            default:
                code = HttpStatusCode.InternalServerError; // 500
                result = JsonSerializer.Serialize(new { error = "An unexpected error has occurred." });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}