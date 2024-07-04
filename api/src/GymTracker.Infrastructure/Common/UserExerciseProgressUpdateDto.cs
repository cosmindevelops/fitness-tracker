namespace GymTracker.Infrastructure.Common;

public class UserExerciseProgressUpdateDto
{
    public int? Set1Reps { get; set; }
    public int? Set2Reps { get; set; }
    public int? Set3Reps { get; set; }
    public int? Set4Reps { get; set; }
    public bool WorkoutCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
}