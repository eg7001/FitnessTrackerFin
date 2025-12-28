namespace FitnessTracker.DTOs.Set
{
    public record SetDto(
        int Id,
        int Reps,
        decimal Weight,
        bool IsFailure
    );
}
