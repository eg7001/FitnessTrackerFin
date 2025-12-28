using FitnessTracker.DTOs.WorkoutExercise;

namespace FitnessTracker.DTOs.Workout
{
    public record UpdateWorkoutDto(
        string Name,
        DateTime? Date
    );
}
