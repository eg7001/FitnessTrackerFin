namespace FitnessTracker.DTOs.Exercise
{
    public record AddExerciseDto(string Name, int Sets, int Repetitions, double? Weight = null);
}
