namespace GymTracker.Infrastructure.Common;

public class UserExerciseProgressResponseDto
{
    public Guid Id { get; set; }
    public Guid UserWorkoutTemplateId { get; set; }
    public Guid TemplateExerciseId { get; set; }
    public int? Set1Reps { get; set; }
    public int? Set2Reps { get; set; }
    public int? Set3Reps { get; set; }
    public int? Set4Reps { get; set; }
    public bool WorkoutCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
    public TemplateExerciseResponseDto TemplateExercise { get; set; }
}