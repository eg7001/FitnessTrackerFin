using FitnessTracker.DTOs.WorkoutExercise;

namespace FitnessTracker.DTOs.Exercise
{
    public record ReturnExerciseDto
    (
        int id,
        string Name,
        List<WorkoutExerciseDto> Workout,
        string? MuscleGroup = null,
        bool IsBodyweight = false
    );
}
