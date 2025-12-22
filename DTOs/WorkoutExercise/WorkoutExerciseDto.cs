using FitnessTracker.DTOs.Set;

namespace FitnessTracker.DTOs.WorkoutExercise
{
    public record WorkoutExerciseDto(
        int Id,
        int ExerciseId,
        string ExerciseName,
        string? MuscleGroup,
        bool IsBodyweight,
        List<SetDto> Sets
    );

}
