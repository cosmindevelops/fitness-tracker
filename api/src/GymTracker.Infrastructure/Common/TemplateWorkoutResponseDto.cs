namespace GymTracker.Infrastructure.Common;

public class TemplateWorkoutResponseDto
{
    public Guid Id { get; set; }
    public Guid TemplateWeekId { get; set; }
    public string Name { get; set; }
    public List<TemplateExerciseResponseDto> TemplateExercises { get; set; }
}