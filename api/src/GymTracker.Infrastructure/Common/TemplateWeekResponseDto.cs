namespace GymTracker.Infrastructure.Common;

public class TemplateWeekResponseDto
{
    public Guid Id { get; set; }
    public Guid WorkoutTemplateId { get; set; }
    public int WeekNumber { get; set; }
    public List<TemplateWorkoutResponseDto> TemplateWorkouts { get; set; }
}