namespace GymTracker.Infrastructure.Common;

public class UserWorkoutTemplateResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WorkoutTemplateId { get; set; }
    public DateTime StartDate { get; set; }
    public WorkoutTemplateResponseDto WorkoutTemplate { get; set; }
    public List<UserExerciseProgressResponseDto> UserExerciseProgress { get; set; }
}