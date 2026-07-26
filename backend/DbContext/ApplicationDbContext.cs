using FitnessTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace FitnessTracker.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,IdentityRole<Guid>,Guid>
    {
        public DbSet<Workout> Workouts => Set<Workout>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
        public DbSet<Set> Sets => Set<Set>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Workout → WorkoutExercise (CASCADE)
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exercise → WorkoutExercise (RESTRICT)
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkoutExercise → Set (CASCADE)
            modelBuilder.Entity<Set>()
                .HasOne(s => s.WorkoutExercise)
                .WithMany(we => we.Sets)
                .HasForeignKey(s => s.WorkoutExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Workout fields
            modelBuilder.Entity<Workout>()
                .Property(w => w.Name)
                .HasMaxLength(100)
                .IsRequired();

            // Exercise fields
            modelBuilder.Entity<Exercise>()
                .Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            // Prevent duplicate exercise names
            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.Name)
                .IsUnique();

            // Exercise creation is admin-only (see ExercisesController), and
            // the only way to become admin is being the first user to ever
            // register - so a fresh database has no way to populate the
            // catalog without this. Seeding it here means every environment
            // (local, Docker, CI) gets the same starter catalog for free via
            // migrations, with no manual admin bootstrapping required.
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { Id = 1, Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false },
                new Exercise { Id = 2, Name = "Incline Bench Press", MuscleGroup = "Chest", IsBodyweight = false },
                new Exercise { Id = 3, Name = "Push-Up", MuscleGroup = "Chest", IsBodyweight = true },
                new Exercise { Id = 4, Name = "Chest Fly", MuscleGroup = "Chest", IsBodyweight = false },
                new Exercise { Id = 5, Name = "Squat", MuscleGroup = "Legs", IsBodyweight = false },
                new Exercise { Id = 6, Name = "Leg Press", MuscleGroup = "Legs", IsBodyweight = false },
                new Exercise { Id = 7, Name = "Romanian Deadlift", MuscleGroup = "Legs", IsBodyweight = false },
                new Exercise { Id = 8, Name = "Lunge", MuscleGroup = "Legs", IsBodyweight = true },
                new Exercise { Id = 9, Name = "Calf Raise", MuscleGroup = "Legs", IsBodyweight = false },
                new Exercise { Id = 10, Name = "Hip Thrust", MuscleGroup = "Legs", IsBodyweight = false },
                new Exercise { Id = 11, Name = "Deadlift", MuscleGroup = "Back", IsBodyweight = false },
                new Exercise { Id = 12, Name = "Barbell Row", MuscleGroup = "Back", IsBodyweight = false },
                new Exercise { Id = 13, Name = "Dumbbell Row", MuscleGroup = "Back", IsBodyweight = false },
                new Exercise { Id = 14, Name = "Pull-Up", MuscleGroup = "Back", IsBodyweight = true },
                new Exercise { Id = 15, Name = "Lat Pulldown", MuscleGroup = "Back", IsBodyweight = false },
                new Exercise { Id = 16, Name = "Overhead Press", MuscleGroup = "Shoulders", IsBodyweight = false },
                new Exercise { Id = 17, Name = "Dumbbell Shoulder Press", MuscleGroup = "Shoulders", IsBodyweight = false },
                new Exercise { Id = 18, Name = "Lateral Raise", MuscleGroup = "Shoulders", IsBodyweight = false },
                new Exercise { Id = 19, Name = "Bicep Curl", MuscleGroup = "Arms", IsBodyweight = false },
                new Exercise { Id = 20, Name = "Tricep Dip", MuscleGroup = "Arms", IsBodyweight = true },
                new Exercise { Id = 21, Name = "Plank", MuscleGroup = "Core", IsBodyweight = true },
                new Exercise { Id = 22, Name = "Sit-Up", MuscleGroup = "Core", IsBodyweight = true }
            );
        }


    }
}
