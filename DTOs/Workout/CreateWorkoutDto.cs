namespace FitnessTracker.DTOs.Workout
{
    public record CreateWorkoutDto(
        string Name,
        DateTime? Date
     );
}
