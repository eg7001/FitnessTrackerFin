namespace FitnessTracker.DTOs.QueryObject
{
    public class ExerciseQueryDto : PaginationDto
    {
        public string? MuscleGroup { get; set; }
        public string? Search { get; set; }
    }
}
