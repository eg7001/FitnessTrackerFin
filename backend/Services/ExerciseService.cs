using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;
        public ExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateExercise(CreateExerciseDto exerciseDto)
        {
            var exercise = new Exercise
            {
                Name = exerciseDto.Name,
                MuscleGroup = exerciseDto.MuscleGroup,
                IsBodyweight = exerciseDto.IsBodyweight

            };
            await _context.AddAsync(exercise);
            await _context.SaveChangesAsync();
        }

            public async Task<ExerciseDto> GetExerciseById(int id)
            {
                var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id);
                if (exercise == null) {
                    throw new Exception("The exercise does not exist");
                }
                var toReturn = new ExerciseDto
                (
                     exercise.Name,
                     exercise.MuscleGroup,
                     exercise.IsBodyweight
                );
                return toReturn;
            }

        public async Task<List<ExerciseDto>> GetExercises()
        {
            var allExercises = await _context.Exercises
                .AsNoTracking()
                .OrderBy(e => e.MuscleGroup)
                .Include(e => e.WorkoutExercises)
                .ThenInclude(w => w.Id)
                .ToListAsync();
            ///var results = allExercises.Select(e => new ReturnExerciseDto(
                
               /// );
            throw new NotImplementedException();
        }
    }
}
