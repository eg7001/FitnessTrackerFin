namespace FitnessTracker.DTOs.Exercise
{
    public record AddExerciseDto(string Name, int Sets, int Repetitions, bool IsBodyweight, string? MuscleGroup, double? Weight = null);
}
