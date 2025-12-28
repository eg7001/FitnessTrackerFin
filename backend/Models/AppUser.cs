using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();


    }
}
