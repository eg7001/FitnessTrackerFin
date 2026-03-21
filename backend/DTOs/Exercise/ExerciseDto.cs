using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.DTOs.Workout
{
    public record ExerciseDto(
        [Required]
        [MaxLength(100)]
        string Name,
        string? MuscleGroup = null,
        bool IsBodyweight = false
    );

}
