namespace GymTracker.Infrastructure.Common;

public class SeriesResponseDto
{
    public Guid Id { get; set; }
    public int Repetitions { get; set; }
    public int RPE { get; set; }
    public double Weight { get; set; }
}