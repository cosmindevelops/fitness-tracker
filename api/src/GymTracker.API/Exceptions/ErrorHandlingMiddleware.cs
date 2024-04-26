using GymTracker.Infrastructure.Common.Exceptions;
using GymTracker.Infrastructure.Exceptions;
using System.Net;

namespace GymTracker.API.Exceptions;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case WorkoutNotFoundException:
                code = HttpStatusCode.NotFound;
                result = $"Error: {exception.Message}";
                break;

            case UserNotFoundException:
                code = HttpStatusCode.NotFound;
                result = $"Error: {exception.Message}";
                break;

            case ExerciseNotFoundException:
                code = HttpStatusCode.NotFound;
                result = $"Error: {exception.Message}";
                break;

            case SeriesNotFoundException:
                code = HttpStatusCode.NotFound;
                result = $"Error: {exception.Message}";
                break;

            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                result = $"Error: {exception.Message}";
                break;

            default:
                result = "An unexpected error has occurred.";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}