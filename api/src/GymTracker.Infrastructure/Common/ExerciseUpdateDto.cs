namespace GymTracker.Infrastructure.Common;

public class ExerciseUpdateDto
{
    public string Name { get; set; }
    public List<SeriesUpdateDto> Series { get; set; } = new List<SeriesUpdateDto>();
}