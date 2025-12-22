namespace FitnessTracker.DTOs.Workout
{
    public record UpdateWorkoutDto(
        string Name,
        DateTime Date,
        List<ExerciseDto> Exercises
    );
}
