namespace GymTracker.Infrastructure.Common;

public class WorkoutResponseDto
{
    public Guid Id { get; set; }
    public string Notes { get; set; }
    public DateTime Date { get; set; }
    public List<ExerciseResponseDto> Exercises { get; set; }
}