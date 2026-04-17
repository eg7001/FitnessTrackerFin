using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Set;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Services
{
    public class SetService : ISetService
    {
        private readonly ApplicationDbContext _context;
        public SetService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddSetToWorkoutExercise(
            Guid userId,
            int workoutExerciseId,
            AddSetDto dto)
        {
            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we =>
                    we.Id == workoutExerciseId &&
                    we.Workout.UserId == userId);
            if (workoutExercise == null)
                throw new UnauthorizedAccessException("WorkoutExercise not found or access denied");

            var set = new Set
            {
                WorkoutExerciseId = workoutExerciseId,
                Reps = dto.Reps,
                Weight = dto.Weight,
                IsFailure = dto.IsFailure
            };

            _context.Sets.Add(set);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteSet(Guid userId, int setId)
        {
            var set = await _context.Sets
               .Include(s => s.WorkoutExercise)
                   .ThenInclude(we => we.Workout)
               .FirstOrDefaultAsync(s =>
                   s.Id == setId &&
                   s.WorkoutExercise.Workout.UserId == userId);

            if (set == null)
            {
                throw new UnauthorizedAccessException("No");
            }
            _context.Sets.Remove(set);
            await _context.SaveChangesAsync();
            return;
        }
        public async Task UpdateSet(Guid userId, int setId, UpdateSetDto dto)
        {
            var set = await _context.Sets
               .Include(s => s.WorkoutExercise)
                   .ThenInclude(we => we.Workout)
               .FirstOrDefaultAsync(s =>
                   s.Id == setId &&
                   s.WorkoutExercise.Workout.UserId == userId);

            if (set == null)
                throw new UnauthorizedAccessException("Access denied or set not found");

            set.Reps = dto.Reps;
            set.Weight = dto.Weight;
            set.IsFailure = dto.IsFailure;

            await _context.SaveChangesAsync();
        }
    }
}
