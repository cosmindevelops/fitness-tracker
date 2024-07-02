namespace GymTracker.Core.Entities;

public class WorkoutTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int DurationWeeks { get; set; }
    public string Description { get; set; }

    // Navigation Property
    public List<TemplateWeek> TemplateWeeks { get; set; } = new List<TemplateWeek>();
}