namespace GymTracker.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public List<Workout> Workouts { get; set; } = new List<Workout>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public List<UserWorkoutTemplate> UserWorkoutTemplates { get; set; } = new List<UserWorkoutTemplate>();
}