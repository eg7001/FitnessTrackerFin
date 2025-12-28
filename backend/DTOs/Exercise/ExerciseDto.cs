namespace FitnessTracker.DTOs.Workout
{
    public record ExerciseDto(
        int Id,
        string Name,
        int Sets,
        int Repetitions,
        double? Weight = null,
        string? MuscleGroup = null,
        bool IsBodyweight = false
    );

}
