namespace GymTracker.Infrastructure.Common.Exceptions;

public class UserWorkoutTemplateNotFoundException : Exception
{
    public UserWorkoutTemplateNotFoundException(Guid templateId, Guid userId) : base($"UserWorkoutTemplate with ID {templateId} not found for user {userId}.")
    {
    }

    public UserWorkoutTemplateNotFoundException(Guid templateId) : base($"UserWorkoutTemplate with ID {templateId} not found.")
    {
    }
}