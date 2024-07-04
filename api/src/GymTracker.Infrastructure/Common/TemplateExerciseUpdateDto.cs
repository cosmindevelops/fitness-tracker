namespace GymTracker.Infrastructure.Common;

public class TemplateExerciseUpdateDto
{
    public string ExerciseName { get; set; }
    public string LastSetIntensity { get; set; }
    public string WarmupSets { get; set; }
    public string WorkingSets { get; set; }
    public string Reps { get; set; }
    public string Rpe { get; set; }
    public string Rest { get; set; }
    public string Substitution1 { get; set; }
    public string Substitution2 { get; set; }
    public string Notes { get; set; }
}