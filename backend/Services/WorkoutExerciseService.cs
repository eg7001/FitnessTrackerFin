using FitnessTracker.DbContext;
using FitnessTracker.DTOs.WorkoutExercise;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Services
{
    public class WorkoutExerciseService : IWorkoutExerciseService
    {
        private readonly ApplicationDbContext _context;
        public WorkoutExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddExerciseToWorkout(
            Guid userId,
            Guid workoutId,
            AddWorkoutExerciseDto dto)
        {
            var workout = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
            if (workout == null)
                throw new UnauthorizedAccessException();
            // Check if Exercise already exists globally
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == dto.ExerciseId);

            if (exercise == null)
            {
                throw new Exception("Exerise was not found");
            }
            // Check if the exercise is already added to this workout
            if (workout.WorkoutExercises.Any(we => we.ExerciseId == dto.ExerciseId))
                throw new Exception("Exercise already added to this workout");
            var workoutExercise = new WorkoutExercise
            {
                WorkoutId = workoutId,
                ExerciseId = dto.ExerciseId
            };

            _context.WorkoutExercises.Add(workoutExercise);
            await _context.SaveChangesAsync();
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
    }
}
