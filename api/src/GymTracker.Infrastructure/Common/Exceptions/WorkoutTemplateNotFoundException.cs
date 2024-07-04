namespace GymTracker.Infrastructure.Common.Exceptions;

public class WorkoutTemplateNotFoundException : Exception
{
    public WorkoutTemplateNotFoundException(Guid templateId) : base($"Workout template with ID {templateId} not found.")
    {
    }
}