using FitnessTracker.DTOs.WorkoutExercise;

namespace FitnessTracker.DTOs.Workout
{
    public record WorkoutDetailsDto(
    Guid Id,
    string Name,
    DateTime Date,
    IReadOnlyList<WorkoutExerciseDto> Exercises
);
}
