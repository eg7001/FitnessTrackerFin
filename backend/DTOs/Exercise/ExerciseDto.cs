namespace FitnessTracker.DTOs.Workout
{
    public record ExerciseDto(
        string Name,
        string? MuscleGroup = null,
        bool IsBodyweight = false
    );

}
