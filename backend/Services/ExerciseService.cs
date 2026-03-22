using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.QueryObject;
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

        public async Task DeleteExercise(int id)
        {
            var exer = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id);
            if(exer == null)
            {
                throw new KeyNotFoundException("The Exercise was not found");
            }
            _context.Exercises.Remove(exer);
            await _context.SaveChangesAsync();  
        }

        public async Task<ExerciseDto> GetExerciseById(int id)
            {
                var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id);
                if (exercise == null) {
                    throw new KeyNotFoundException("The Exercise was not found");
            }
                var toReturn = new ExerciseDto
                (
                     exercise.Name,
                     exercise.MuscleGroup,
                     exercise.IsBodyweight
                );
                return toReturn;
        }

        public async Task<List<ReturnExerciseDto>> GetExercises(ExerciseQueryDto dto)
        {
            var query = _context.Exercises
                .AsNoTracking()
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(e => e.Name.Contains(dto.Search));
            }
            if (!string.IsNullOrWhiteSpace(dto.MuscleGroup)) {
                query = query.Where(e => e.MuscleGroup == dto.MuscleGroup);
            }

            var allExercises = await query
                .OrderBy(e => e.MuscleGroup)
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();
            var results = allExercises.Select(e => new ReturnExerciseDto(
                e.Id,
                e.Name,
                e.MuscleGroup,
                e.IsBodyweight
                )).ToList();
            return results;
        }

        public async Task<ReturnExerciseDto> UpdateExercise(int id, ExerciseDto dto)
        {
            var exer = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id);
            if (exer == null)
            {
                throw new Exception("The exercise does not exist");
            }
            exer.Name = dto.Name;
            exer.MuscleGroup = dto.MuscleGroup;
            exer.IsBodyweight = dto.IsBodyweight;
            await _context.SaveChangesAsync();
            return new ReturnExerciseDto(
                id,
                dto.Name,
                dto.MuscleGroup,
                dto.IsBodyweight);
        }
    }
}
