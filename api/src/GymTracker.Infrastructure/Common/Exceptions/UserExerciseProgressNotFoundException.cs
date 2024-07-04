namespace GymTracker.Infrastructure.Common.Exceptions;

public class UserExerciseProgressNotFoundException : Exception
{
    public UserExerciseProgressNotFoundException(Guid progressId) : base($"User exercise progress with ID {progressId} not found.")
    {
    }
}