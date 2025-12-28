namespace FitnessTracker.DTOs.Set
{
    public record AddSetDto(
        int Reps,
        decimal Weight,
        bool IsFailure
    );
}
