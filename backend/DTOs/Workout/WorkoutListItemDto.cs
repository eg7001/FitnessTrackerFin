namespace FitnessTracker.DTOs.Workout
{
    public record WorkoutListItemDto(
        Guid Id,
        string Name,
        DateTime Date,
        int ExerciseCount
    );
}
