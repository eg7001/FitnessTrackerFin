namespace FitnessTracker.Models
{
    public class Set
    {
        public int Id { get; set; }

        public int WorkoutExerciseId { get; set; }
        public WorkoutExercise WorkoutExercise { get; set; } = null!;

        public int Reps { get; set; }

        public decimal Weight { get; set; }

        public bool IsFailure { get; set; }
    }

}
