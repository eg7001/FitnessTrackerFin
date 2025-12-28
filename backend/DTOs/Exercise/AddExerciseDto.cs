namespace FitnessTracker.DTOs.Exercise
{
    public record AddExerciseDto(
        string Name,
        string? MuscleGroup,
        bool IsBodyweight
    );
}
