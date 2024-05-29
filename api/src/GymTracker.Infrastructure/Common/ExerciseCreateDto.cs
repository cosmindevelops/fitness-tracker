namespace GymTracker.Infrastructure.Common;

public class ExerciseCreateDto
{
    public string Name { get; set; }
    public List<SeriesCreateDto>? Series { get; set; }
}