namespace GymTracker.Core.Entities;

public class Series
{
    public Guid Id { get; set; }
    public int Repetitions { get; set; }
    public int RPE { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
}