using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Workout;

namespace FitnessTracker.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task<WorkoutDto> CreateWorkout(Guid userId, CreateWorkoutDto dto);
        Task<List<WorkoutDto>> GetUserWorkouts(Guid userId);
        Task AddExerciseToWorkout(Guid workoutId, AddExerciseDto dto);
    }
}
