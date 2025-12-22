using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Set;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.DTOs.WorkoutExercise;

namespace FitnessTracker.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task<WorkoutDto> CreateWorkout(Guid userId, CreateWorkoutDto dto);
        Task<List<WorkoutDto>> GetUserWorkouts(Guid userId, int page = 1, int pageSize = 10);
        Task DeleteWorkout(Guid userId, Guid workoutId);
        Task<WorkoutDto> UpdateWorkout(Guid userid,Guid workoutId);

        Task AddExerciseToWorkout(Guid userid,Guid workoutId, AddWorkoutExerciseDto dto);
        Task AddSetToWorkoutExercise(Guid userId,int workoutExerciseId, AddSetDto dto);
    }
}
