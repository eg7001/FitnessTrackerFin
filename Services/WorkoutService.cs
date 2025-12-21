using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;
        public WorkoutService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddExerciseToWorkout(Guid userId,Guid workoutId, AddExerciseDto dto)
        {
            var workout = await _context.Workouts
                .Include(w => w.Exercises)
                .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

            if (workout == null)
                throw new UnauthorizedAccessException("Workout not found or access denied");

            // Check if Exercise already exists globally, or create it
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Name == dto.Name);

            var alreadyExists = workout.Exercises.Any(we =>
                    we.Exercise.Name == dto.Name);
            if (alreadyExists)
                throw new Exception("This exercise is already added to the workout");

            if (exercise == null)
            {
                exercise = new Exercise
                {
                    Name = dto.Name,
                    MuscleGroup = dto.MuscleGroup,
                    IsBodyweight = dto.IsBodyweight
                };
                _context.Exercises.Add(exercise);
            }
            
            var workoutExercise = new WorkoutExercise
            {
                Workout = workout,
                Exercise = exercise,
                Sets = dto.Sets,
                Repetitions = dto.Repetitions,
                Weight = dto.Weight
            };
            _context.WorkoutExercises.Add(workoutExercise);
            await _context.SaveChangesAsync();
        }

        public async Task<WorkoutDto> CreateWorkout(Guid userId, CreateWorkoutDto dto)
        {
            var workout = new Workout
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Date = dto.Date ?? DateTime.UtcNow,
                UserId = userId
            };

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return new WorkoutDto(workout.Id, dto.Name, workout.Date, new List<ExerciseDto>());
        }

        public async Task DeleteWorkout(Guid userId, Guid workoutId)
        {
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == workoutId);
            if (workout == null)
            {
                throw new UnauthorizedAccessException("Workout not found or unaothrized access");
            }
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<List<WorkoutDto>> GetUserWorkouts(
            Guid userId,
            int page = 1, 
            int pageSize = 10)
        {
            var workouts = await _context.Workouts
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Skip((page -1) * pageSize)
                .Take(pageSize)
                .Include(w => w.Exercises)
                    .ThenInclude(we => we.Exercise)
                .ToListAsync();
            return workouts.Select(w => new WorkoutDto(
                w.Id,
                w.Name,
                w.Date,
                w.Exercises.Select(we => new ExerciseDto(
                    we.Exercise.Id,
                    we.Exercise.Name,
                    we.Sets,
                    we.Repetitions,
                    we.Weight,
                    we.Exercise.MuscleGroup,
                    we.Exercise.IsBodyweight
                )).ToList()
            )).ToList();
        }
    }
}
