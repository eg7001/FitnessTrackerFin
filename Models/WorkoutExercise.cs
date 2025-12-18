using System.Collections.Generic;

namespace FitnessTracker.Models
{
    public class WorkoutExercise
    {
        public Guid WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public double? Weight { get; set; }
    }
}

