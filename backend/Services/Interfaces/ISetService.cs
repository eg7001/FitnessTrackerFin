using FitnessTracker.DTOs.Set;

namespace FitnessTracker.Services.Interfaces
{
    public interface ISetService
    {
        Task AddSetToWorkoutExercise(Guid userId, int workoutExerciseId, AddSetDto dto);
        Task UpdateSet(Guid userId, int setId, UpdateSetDto dto);
        Task DeleteSet(Guid userId, int setId);
    }
}
