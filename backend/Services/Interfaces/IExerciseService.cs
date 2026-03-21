using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Workout;

namespace FitnessTracker.Services.Interfaces
{
    public interface IExerciseService
    {
        Task CreateExercise(CreateExerciseDto exercise);
        Task<List<ReturnExerciseDto>> GetExercises();
        Task<ExerciseDto> GetExerciseById(int id);

    }
}
