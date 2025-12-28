namespace FitnessTracker.DTOs.WorkoutExercise
{
    public record AddWorkoutExerciseDto(
        string ExerciseName,
        string? MuscleGroup,
        bool IsBodyweight
    );
}
