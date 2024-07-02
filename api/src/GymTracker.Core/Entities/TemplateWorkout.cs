namespace GymTracker.Core.Entities;

public class TemplateWorkout
{
    public Guid Id { get; set; }
    public Guid TemplateWeekId { get; set; }
    public string Name { get; set; }

    // Navigation properties
    public TemplateWeek TemplateWeek { get; set; }

    public List<TemplateExercise> TemplateExercises { get; set; } = new List<TemplateExercise>();
}