namespace FitnessTracker.Models
{
    public class Workout
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public ICollection<WorkoutExercise> Exercises { get; set; } = new List<WorkoutExercise>();
    }

}
