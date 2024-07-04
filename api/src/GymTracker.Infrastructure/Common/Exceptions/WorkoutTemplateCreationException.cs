namespace GymTracker.Infrastructure.Common.Exceptions;

public class WorkoutTemplateCreationException : Exception
{
    public WorkoutTemplateCreationException(string message) : base($"Error creating workout template: {message}")
    {
    }
}