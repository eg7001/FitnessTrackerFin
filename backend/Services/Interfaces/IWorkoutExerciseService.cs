using FitnessTracker.DTOs.WorkoutExercise;

namespace FitnessTracker.Services.Interfaces
{
    public interface IWorkoutExerciseService
    {
        Task AddExerciseToWorkout(Guid userid, Guid workoutId, AddWorkoutExerciseDto dto);
        Task DeleteWorkoutExercise(Guid userid, int workoutExercsieId);

    }
}
