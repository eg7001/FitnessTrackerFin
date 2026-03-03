using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Workout;

namespace FitnessTracker.Services.Interfaces
{
    public interface IExerciseService
    {
        Task CreateExercise(CreateExerciseDto exercise);
        Task<List<ExerciseDto>> GetExercises();
        Task<ExerciseDto> GetExerciseById(int id);
    }
}
