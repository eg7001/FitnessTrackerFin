using FitnessTracker.DbContext;
using FitnessTracker.DTOs.WorkoutExercise;
using FitnessTracker.Models;
using FitnessTracker.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitnessTracker.Tests.Services;

public class WorkoutExerciseServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddExerciseToWorkout_WhenValid_AddsAndReturnsDto()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workout = new Workout { Id = Guid.NewGuid(), UserId = userId, Name = "Push Day", Date = DateTime.UtcNow };
        var exercise = new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false };
        context.Workouts.Add(workout);
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var sut = new WorkoutExerciseService(context);
        var result = await sut.AddExerciseToWorkout(userId, workout.Id, new AddWorkoutExerciseDto(exercise.Id));

        Assert.Equal("Bench Press", result.ExerciseName);
        Assert.Empty(result.Sets);
        Assert.Single(context.WorkoutExercises);
    }

    [Fact]
    public async Task AddExerciseToWorkout_WhenWorkoutNotOwnedByUser_ThrowsUnauthorizedAccessException()
    {
        var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var workout = new Workout { Id = Guid.NewGuid(), UserId = ownerId, Name = "Push Day", Date = DateTime.UtcNow };
        var exercise = new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false };
        context.Workouts.Add(workout);
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var sut = new WorkoutExerciseService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.AddExerciseToWorkout(Guid.NewGuid(), workout.Id, new AddWorkoutExerciseDto(exercise.Id)));
    }

    [Fact]
    public async Task AddExerciseToWorkout_WhenExerciseDoesNotExist_ThrowsException()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workout = new Workout { Id = Guid.NewGuid(), UserId = userId, Name = "Push Day", Date = DateTime.UtcNow };
        context.Workouts.Add(workout);
        await context.SaveChangesAsync();

        var sut = new WorkoutExerciseService(context);

        await Assert.ThrowsAsync<Exception>(
            () => sut.AddExerciseToWorkout(userId, workout.Id, new AddWorkoutExerciseDto(999)));
    }

    [Fact]
    public async Task AddExerciseToWorkout_WhenExerciseAlreadyInWorkout_ThrowsException()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workout = new Workout { Id = Guid.NewGuid(), UserId = userId, Name = "Push Day", Date = DateTime.UtcNow };
        var exercise = new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false };
        context.Workouts.Add(workout);
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();
        context.WorkoutExercises.Add(new WorkoutExercise { WorkoutId = workout.Id, ExerciseId = exercise.Id });
        await context.SaveChangesAsync();

        var sut = new WorkoutExerciseService(context);

        await Assert.ThrowsAsync<Exception>(
            () => sut.AddExerciseToWorkout(userId, workout.Id, new AddWorkoutExerciseDto(exercise.Id)));
    }

    [Fact]
    public async Task DeleteWorkoutExercise_WhenOwnedByUser_RemovesIt()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workout = new Workout { Id = Guid.NewGuid(), UserId = userId, Name = "Push Day", Date = DateTime.UtcNow };
        var exercise = new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false };
        context.Workouts.Add(workout);
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();
        var workoutExercise = new WorkoutExercise { WorkoutId = workout.Id, ExerciseId = exercise.Id };
        context.WorkoutExercises.Add(workoutExercise);
        await context.SaveChangesAsync();

        var sut = new WorkoutExerciseService(context);
        await sut.DeleteWorkoutExercise(userId, workoutExercise.Id);

        Assert.Empty(context.WorkoutExercises);
    }

    [Fact]
    public async Task DeleteWorkoutExercise_WhenNotOwnedByUser_ThrowsUnauthorizedAccessException()
    {
        var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var workout = new Workout { Id = Guid.NewGuid(), UserId = ownerId, Name = "Push Day", Date = DateTime.UtcNow };
        var exercise = new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false };
        context.Workouts.Add(workout);
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();
        var workoutExercise = new WorkoutExercise { WorkoutId = workout.Id, ExerciseId = exercise.Id };
        context.WorkoutExercises.Add(workoutExercise);
        await context.SaveChangesAsync();

        var sut = new WorkoutExerciseService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.DeleteWorkoutExercise(Guid.NewGuid(), workoutExercise.Id));
    }
}
