namespace FitnessTracker.DTOs.Workout;
using FitnessTracker.DTOs.Exercise;

public record WorkoutDto(
    Guid Id,
    string Name,
    DateTime Date,
    List<ExerciseDto> Exercises
);
