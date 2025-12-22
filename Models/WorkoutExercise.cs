using System.Collections.Generic;

namespace FitnessTracker.Models
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        public Guid WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        public ICollection<Set> Sets { get; set; }
            = new List<Set>();
    }
}

