using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Set;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.DTOs.WorkoutExercise;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FitnessTracker.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;
        public WorkoutService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<WorkoutDto>> GetUserWorkouts(
             Guid userId,
             int page = 1,
             int pageSize = 10)
        {
            // Step 1: Query the database with proper Includes
            var workouts = await _context.Workouts
                .AsNoTracking()
                .Where(w => w.UserId == userId) // Only the current user's workouts
                .OrderByDescending(w => w.Date) // Latest first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(w => w.WorkoutExercises)           // Include the join table
                    .ThenInclude(we => we.Exercise)        // Include Exercise info
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Sets)            // Include Sets
                .ToListAsync();

            // Step 2: Map EF models to DTOs
            var result = workouts.Select(w => new WorkoutDto(
                w.Id,
                w.Name,
                w.Date,
                w.WorkoutExercises.Select(we => new WorkoutExerciseDto(
                    we.Id,                    // WorkoutExercise ID
                    we.Exercise.Id,           // ExerciseId
                    we.Exercise.Name,         // ExerciseName
                    we.Exercise.MuscleGroup,  // MuscleGroup
                    we.Exercise.IsBodyweight, // IsBodyweight
                    we.Sets.Select(s => new SetDto(
                        s.Id,
                        s.Reps,
                        s.Weight,
                        s.IsFailure
                    )).ToList()
                )).ToList()
            )).ToList();

            return result;
        }
        public async Task<WorkoutDto> GetWorkoutById(Guid userId, Guid workoutId)
        {
            var workout = await _context.Workouts
                .AsNoTracking()
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Sets)
                .FirstOrDefaultAsync(w =>
                    w.Id == workoutId &&
                    w.UserId == userId
                );

            if (workout == null)
                throw new UnauthorizedAccessException("Workout not found or access denied");

            return new WorkoutDto(
                workout.Id,
                workout.Name,
                workout.Date,
                workout.WorkoutExercises.Select(we => new WorkoutExerciseDto(
                    we.Id,
                    we.ExerciseId,
                    we.Exercise.Name,
                    we.Exercise.MuscleGroup,
                    we.Exercise.IsBodyweight,
                    we.Sets.Select(s => new SetDto(
                        s.Id,
                        s.Reps,
                        s.Weight,
                        s.IsFailure
                    )).ToList()
                )).ToList()
            );
        }
        public async Task AddExerciseToWorkout(
             Guid userId,
             Guid workoutId,
             AddWorkoutExerciseDto dto)
        {
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
            if (workout == null)
                throw new UnauthorizedAccessException();
            // Check if Exercise already exists globally
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Name == dto.ExerciseName); 
            // Check if the exercise is already added to this workout
            if (workout.WorkoutExercises.Any(we => we.Exercise.Name == dto.ExerciseName))
                throw new Exception("Exercise already added to this workout");

            if (exercise == null)
            {
                exercise = new Exercise
                {
                    Name = dto.ExerciseName,
                    MuscleGroup = dto.MuscleGroup,
                    IsBodyweight = dto.IsBodyweight
                };
                _context.Exercises.Add(exercise);
            }
            var workoutExercise = new WorkoutExercise
            {
                WorkoutId = workoutId,
                Exercise = exercise
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
            return new WorkoutDto(workout.Id, dto.Name, workout.Date, new List<WorkoutExerciseDto>());
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
        public async Task DeleteWorkoutExercise(Guid userid, int workoutExercsieId)
        {
            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we =>
                    we.Id == workoutExercsieId &&
                    we.Workout.UserId == userid);
            if (workoutExercise == null)
                throw new UnauthorizedAccessException("");
            _context.WorkoutExercises.Remove(workoutExercise); 
            await _context.SaveChangesAsync();
        }
        public async Task<WorkoutDto> UpdateWorkout(Guid userid, Guid workoutId, UpdateWorkoutDto dto)
        {
            var workout =  await _context.Workouts
                .FirstOrDefaultAsync(w => w.UserId == userid && w.Id == workoutId);

            if (workout == null)
            {
                throw new UnauthorizedAccessException("You have no access or the workout does not exist");
            }
            workout.Name = dto.Name;
            workout.Date = dto.Date ?? workout.Date;
            await _context.SaveChangesAsync();
            return new WorkoutDto(
                   workoutId,
                   workout.Name,
                   workout.Date,
                   new List<WorkoutExerciseDto>()
             );  
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

            if (set != null)
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
