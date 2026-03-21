using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.DTOs.Set
{
    public record SetDto(
        int Id,
        [Range(1, int.MaxValue)]
        int Reps,
        [Range(0.1, double.MaxValue)]
        decimal Weight,
        bool IsFailure
    );
}
