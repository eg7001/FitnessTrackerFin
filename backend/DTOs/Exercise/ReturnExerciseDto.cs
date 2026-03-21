using FitnessTracker.DTOs.WorkoutExercise;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.DTOs.Exercise
{
    public record ReturnExerciseDto
    (
        int id,
        [Required]
        [MaxLength(100)]
        string Name,
        string? MuscleGroup = null,
        bool IsBodyweight = false
    );
}
