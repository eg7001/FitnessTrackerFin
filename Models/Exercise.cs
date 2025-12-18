namespace FitnessTracker.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? MuscleGroup { get; set; }

        public bool IsBodyweight { get; set; }

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }

}
