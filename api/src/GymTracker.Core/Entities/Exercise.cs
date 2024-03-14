namespace GymTracker.Core.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid WorkoutId { get; set; }
    public Workout Workout { get; set; }
    public List<Series> Series { get; set; } = new List<Series>();
}