using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.DTOs.Set
{
    public record UpdateSetDto(
        [Range(1, int.MaxValue)]
        int Reps,
        [Range(0.1, double.MaxValue)]
        decimal Weight,
        bool IsFailure
    );
}
