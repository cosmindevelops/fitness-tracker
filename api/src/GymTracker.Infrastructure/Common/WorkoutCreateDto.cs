namespace GymTracker.Infrastructure.Common;

public class WorkoutCreateDto
{
    public string Notes { get; set; }
    public DateTime Date { get; set; }
    public List<ExerciseCreateDto> Exercises { get; set; }
}