namespace FitnessTracker.DTOs.Workout;
using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.WorkoutExercise;

public record WorkoutDto(
    Guid Id,
    string Name,
    DateTime Date,
    List<WorkoutExerciseDto> Exercises
);

