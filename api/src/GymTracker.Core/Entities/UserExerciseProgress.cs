namespace GymTracker.Core.Entities;

public class UserExerciseProgress
{
    public Guid Id { get; set; }
    public Guid UserWorkoutTemplateId { get; set; }
    public Guid TemplateExerciseId { get; set; }
    public int? Set1Reps { get; set; }
    public int? Set2Reps { get; set; }
    public int? Set3Reps { get; set; }
    public int? Set4Reps { get; set; }
    public bool WorkoutCompleted { get; set; } = false;
    public DateTime? CompletionDate { get; set; }

    // Navigation Properties
    public UserWorkoutTemplate UserWorkoutTemplate { get; set; }
    public TemplateExercise TemplateExercise { get; set; }
}