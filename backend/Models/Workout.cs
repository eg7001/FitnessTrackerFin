namespace FitnessTracker.Models
{
    public class Workout
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Date { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
            = new List<WorkoutExercise>();
    }
}
