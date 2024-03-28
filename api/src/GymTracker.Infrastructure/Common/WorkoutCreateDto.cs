using System.ComponentModel.DataAnnotations;

namespace GymTracker.Core.DTOs;

public class WorkoutCreateDto
{
    [Required]
    [StringLength(50)]
    public string Notes { get; set; }

    public DateTime Date { get; set; }
}