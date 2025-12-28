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
        }


    }
}
