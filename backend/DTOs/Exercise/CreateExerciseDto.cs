using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.DTOs.Exercise
{
    public record CreateExerciseDto
    (
        [Required]
        [MaxLength(100)]
        string Name,
        string? MuscleGroup,
        bool IsBodyweight
    );
}
