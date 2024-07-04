namespace GymTracker.Infrastructure.Common.Exceptions;

public class TemplateAlreadySelectedException : Exception
{
    public TemplateAlreadySelectedException(Guid userId, Guid templateId) : base($"User {userId} has already selected template {templateId}.")
    {
    }
}