namespace GymTracker.Infrastructure.Common.Exceptions;

public class ExerciseNotFoundException : Exception
{
    public ExerciseNotFoundException(string message) : base(message)
    {
    }
}