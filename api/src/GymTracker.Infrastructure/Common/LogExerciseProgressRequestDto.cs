namespace GymTracker.Infrastructure.Common;

public class LogExerciseProgressRequestDto
{
    public Guid UserWorkoutTemplateId { get; set; }
    public Guid TemplateExerciseId { get; set; }
    public UserExerciseProgressUpdateDto ProgressDto { get; set; }
}