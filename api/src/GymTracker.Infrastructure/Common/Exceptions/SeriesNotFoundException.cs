namespace GymTracker.Infrastructure.Common.Exceptions;

public class SeriesNotFoundException : Exception
{
    public SeriesNotFoundException(string message) : base(message)
    {
    }
}