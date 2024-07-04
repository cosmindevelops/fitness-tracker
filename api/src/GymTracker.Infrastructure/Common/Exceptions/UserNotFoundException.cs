namespace GymTracker.Infrastructure.Common.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message)
    {
    }

    public UserNotFoundException(Guid userId) : base($"User with ID {userId} not found.")
    {
    }
}