using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Workout;

namespace FitnessTracker.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task<WorkoutDto> CreateWorkout(Guid userId, CreateWorkoutDto dto);
        Task<List<WorkoutDto>> GetUserWorkouts(Guid userId,int page = 1,int pageSize = 10);
        Task AddExerciseToWorkout(Guid userId,Guid workoutId, AddExerciseDto dto);
        Task DeleteWorkout(Guid userId, Guid workoutId);
    }
}
