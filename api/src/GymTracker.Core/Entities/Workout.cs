namespace GymTracker.Core.Entities;

public class Workout
{
    public Guid Id { get; set; }
    public string Notes { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<Exercise> Exercises { get; set; } = new List<Exercise>();
}