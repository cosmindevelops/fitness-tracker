namespace GymTracker.Infrastructure.Common;

public class WorkoutTemplateResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int DurationWeeks { get; set; }
    public string Description { get; set; }
    public List<TemplateWeekResponseDto> TemplateWeeks { get; set; }
}