namespace GymTracker.Core.Entities;

public class TemplateWeek
{
    public Guid Id { get; set; }
    public Guid WorkoutTemplateId { get; set; }
    public int WeekNumber { get; set; }

    // Navigation properties
    public WorkoutTemplate WorkoutTemplate { get; set; }
    public List<TemplateWorkout> TemplateWorkouts { get; set; } = new List<TemplateWorkout>();
}