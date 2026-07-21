using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Set;
using FitnessTracker.Models;
using FitnessTracker.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitnessTracker.Tests.Services;

public class SetServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<(WorkoutExercise workoutExercise, Guid ownerId)> SeedWorkoutExercise(ApplicationDbContext context)
    {
        var ownerId = Guid.NewGuid();
        var exercise = new Exercise { Name = "Squat", MuscleGroup = "Legs", IsBodyweight = false };
        var workout = new Workout { Id = Guid.NewGuid(), UserId = ownerId, Name = "Leg Day", Date = DateTime.UtcNow };
        context.Exercises.Add(exercise);
        context.Workouts.Add(workout);
        await context.SaveChangesAsync();

        var workoutExercise = new WorkoutExercise { WorkoutId = workout.Id, ExerciseId = exercise.Id };
        context.WorkoutExercises.Add(workoutExercise);
        await context.SaveChangesAsync();

        return (workoutExercise, ownerId);
    }

    [Fact]
    public async Task AddSetToWorkoutExercise_WhenOwnedByUser_AddsSetAndReturnsDto()
    {
        var context = CreateContext();
        var (workoutExercise, ownerId) = await SeedWorkoutExercise(context);
        var sut = new SetService(context);

        var result = await sut.AddSetToWorkoutExercise(ownerId, workoutExercise.Id, new AddSetDto(8, 100m, false));

        Assert.Equal(8, result.Reps);
        Assert.Equal(100m, result.Weight);
        Assert.False(result.IsFailure);
        Assert.True(result.Id > 0);
        Assert.Single(context.Sets);
    }

    [Fact]
    public async Task AddSetToWorkoutExercise_WhenNotOwnedByUser_ThrowsUnauthorizedAccessExceptionAndDoesNotAdd()
    {
        var context = CreateContext();
        var (workoutExercise, _) = await SeedWorkoutExercise(context);
        var sut = new SetService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.AddSetToWorkoutExercise(Guid.NewGuid(), workoutExercise.Id, new AddSetDto(8, 100m, false)));
        Assert.Empty(context.Sets);
    }

    [Fact]
    public async Task AddSetToWorkoutExercise_WhenWorkoutExerciseDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        var sut = new SetService(CreateContext());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.AddSetToWorkoutExercise(Guid.NewGuid(), 999, new AddSetDto(8, 100m, false)));
    }

    [Fact]
    public async Task UpdateSet_WhenOwnedByUser_UpdatesFields()
    {
        var context = CreateContext();
        var (workoutExercise, ownerId) = await SeedWorkoutExercise(context);
        var set = new Set { WorkoutExerciseId = workoutExercise.Id, Reps = 5, Weight = 50m, IsFailure = false };
        context.Sets.Add(set);
        await context.SaveChangesAsync();

        var sut = new SetService(context);
        await sut.UpdateSet(ownerId, set.Id, new UpdateSetDto(12, 40m, true));

        var updated = await context.Sets.SingleAsync(s => s.Id == set.Id);
        Assert.Equal(12, updated.Reps);
        Assert.Equal(40m, updated.Weight);
        Assert.True(updated.IsFailure);
    }

    [Fact]
    public async Task UpdateSet_WhenNotOwnedByUser_ThrowsUnauthorizedAccessException()
    {
        var context = CreateContext();
        var (workoutExercise, _) = await SeedWorkoutExercise(context);
        var set = new Set { WorkoutExerciseId = workoutExercise.Id, Reps = 5, Weight = 50m, IsFailure = false };
        context.Sets.Add(set);
        await context.SaveChangesAsync();

        var sut = new SetService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.UpdateSet(Guid.NewGuid(), set.Id, new UpdateSetDto(12, 40m, true)));
    }

    [Fact]
    public async Task DeleteSet_WhenOwnedByUser_RemovesSet()
    {
        var context = CreateContext();
        var (workoutExercise, ownerId) = await SeedWorkoutExercise(context);
        var set = new Set { WorkoutExerciseId = workoutExercise.Id, Reps = 5, Weight = 50m, IsFailure = false };
        context.Sets.Add(set);
        await context.SaveChangesAsync();

        var sut = new SetService(context);
        await sut.DeleteSet(ownerId, set.Id);

        Assert.Empty(context.Sets);
    }

    [Fact]
    public async Task DeleteSet_WhenNotOwnedByUser_ThrowsUnauthorizedAccessExceptionAndDoesNotDelete()
    {
        var context = CreateContext();
        var (workoutExercise, _) = await SeedWorkoutExercise(context);
        var set = new Set { WorkoutExerciseId = workoutExercise.Id, Reps = 5, Weight = 50m, IsFailure = false };
        context.Sets.Add(set);
        await context.SaveChangesAsync();

        var sut = new SetService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.DeleteSet(Guid.NewGuid(), set.Id));
        Assert.Single(context.Sets);
    }
}
