using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.QueryObject;
using FitnessTracker.DTOs.Workout;

namespace FitnessTracker.Services.Interfaces
{
    public interface IExerciseService
    {
        Task CreateExercise(CreateExerciseDto exercise);
        Task<List<ReturnExerciseDto>> GetExercises(ExerciseQueryDto dto);
        Task<ExerciseDto> GetExerciseById(int id);

    }
}
