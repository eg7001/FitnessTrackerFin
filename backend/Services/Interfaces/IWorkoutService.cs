using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.QueryObject;
using FitnessTracker.DTOs.Set;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.DTOs.WorkoutExercise;

namespace FitnessTracker.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task<WorkoutDto> CreateWorkout(Guid userId, CreateWorkoutDto dto);
        Task<List<WorkoutDto>> GetUserWorkouts(Guid userId, PaginationDto dto);
        Task<WorkoutDto> GetWorkoutById(Guid userId, Guid workoutId);
        Task DeleteWorkout(Guid userId, Guid workoutId);
        Task<WorkoutDto> UpdateWorkout(Guid userid, Guid workoutId, UpdateWorkoutDto dto);

    }
}
