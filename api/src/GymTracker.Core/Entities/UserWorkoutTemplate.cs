namespace GymTracker.Core.Entities;

public class UserWorkoutTemplate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WorkoutTemplateId { get; set; }
    public DateTime StartDate { get; set; }

    // Navigation properties
    public User User { get; set; }
    public WorkoutTemplate WorkoutTemplate { get; set; }
    public List<UserExerciseProgress> UserExerciseProgress { get; set; } = new List<UserExerciseProgress>();
}