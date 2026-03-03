namespace FitnessTracker.DTOs.Exercise
{
    public record CreateExerciseDto
    (
        string Name,
        string? MuscleGroup,
        bool IsBodyweight
    );
}
