namespace FitnessTracker.DTOs.Set
{
    public record UpdateSetDto(
        int Reps,
        decimal Weight,
        bool IsFailure
    );
}
