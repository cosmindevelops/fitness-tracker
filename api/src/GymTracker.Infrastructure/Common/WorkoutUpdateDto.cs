namespace GymTracker.Infrastructure.Common;

public class WorkoutUpdateDto
{
    public string Notes { get; set; }
    public DateTime Date { get; set; }
    public List<ExerciseUpdateDto> Exercises { get; set; } = new List<ExerciseUpdateDto>();
}