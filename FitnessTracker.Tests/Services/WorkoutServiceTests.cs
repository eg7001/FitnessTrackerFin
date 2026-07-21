using FitnessTracker.DbContext;
using FitnessTracker.DTOs.QueryObject;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.Models;
using FitnessTracker.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitnessTracker.Tests.Services;

public class WorkoutServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateWorkout_PersistsWorkoutForGivenUser()
    {
        var context = CreateContext();
        var sut = new WorkoutService(context);
        var userId = Guid.NewGuid();

        var result = await sut.CreateWorkout(userId, new CreateWorkoutDto("Leg Day", null));

        Assert.Equal("Leg Day", result.Name);
        var stored = await context.Workouts.SingleAsync();
        Assert.Equal(userId, stored.UserId);
        Assert.Equal("Leg Day", stored.Name);
    }

    [Fact]
    public async Task CreateWorkout_WhenDateNotProvided_DefaultsToNow()
    {
        var context = CreateContext();
        var sut = new WorkoutService(context);
        var before = DateTime.UtcNow;

        var result = await sut.CreateWorkout(Guid.NewGuid(), new CreateWorkoutDto("Push Day", null));

        var after = DateTime.UtcNow;
        Assert.InRange(result.Date, before, after);
    }

    [Fact]
    public async Task GetUserWorkouts_OnlyReturnsWorkoutsBelongingToRequestingUser()
    {
        var context = CreateContext();
        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();
        context.Workouts.AddRange(
            new Workout { Id = Guid.NewGuid(), UserId = userA, Name = "A's workout", Date = DateTime.UtcNow },
            new Workout { Id = Guid.NewGuid(), UserId = userB, Name = "B's workout", Date = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);
        var result = await sut.GetUserWorkouts(userA, new PaginationDto());

        var workout = Assert.Single(result);
        Assert.Equal("A's workout", workout.Name);
    }

    [Fact]
    public async Task GetUserWorkouts_OrdersByDateDescending()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        context.Workouts.AddRange(
            new Workout { Id = Guid.NewGuid(), UserId = userId, Name = "Oldest", Date = DateTime.UtcNow.AddDays(-5) },
            new Workout { Id = Guid.NewGuid(), UserId = userId, Name = "Newest", Date = DateTime.UtcNow },
            new Workout { Id = Guid.NewGuid(), UserId = userId, Name = "Middle", Date = DateTime.UtcNow.AddDays(-2) });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);
        var result = await sut.GetUserWorkouts(userId, new PaginationDto { Page = 1, PageSize = 10 });

        Assert.Equal(new[] { "Newest", "Middle", "Oldest" }, result.Select(w => w.Name));
    }

    [Fact]
    public async Task GetUserWorkouts_RespectsPagination()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            context.Workouts.Add(new Workout
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = $"Workout {i}",
                Date = DateTime.UtcNow.AddDays(-i)
            });
        }
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);
        var page1 = await sut.GetUserWorkouts(userId, new PaginationDto { Page = 1, PageSize = 2 });
        var page2 = await sut.GetUserWorkouts(userId, new PaginationDto { Page = 2, PageSize = 2 });

        Assert.Equal(2, page1.Count);
        Assert.Equal(2, page2.Count);
        Assert.DoesNotContain(page1, w => page2.Select(x => x.Id).Contains(w.Id));
    }

    [Fact]
    public async Task GetWorkoutById_WhenOwnedByUser_ReturnsWorkoutWithExercisesAndSets()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();

        var exercise = new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false };
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        context.Workouts.Add(new Workout { Id = workoutId, UserId = userId, Name = "Push Day", Date = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var workoutExercise = new WorkoutExercise { WorkoutId = workoutId, ExerciseId = exercise.Id };
        context.WorkoutExercises.Add(workoutExercise);
        await context.SaveChangesAsync();

        context.Sets.Add(new Set { WorkoutExerciseId = workoutExercise.Id, Reps = 10, Weight = 60, IsFailure = false });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);
        var result = await sut.GetWorkoutById(userId, workoutId);

        Assert.Equal("Push Day", result.Name);
        var returnedExercise = Assert.Single(result.Exercises);
        Assert.Equal("Bench Press", returnedExercise.ExerciseName);
        var returnedSet = Assert.Single(returnedExercise.Sets);
        Assert.Equal(10, returnedSet.Reps);
    }

    [Fact]
    public async Task GetWorkoutById_WhenNotOwnedByUser_ThrowsUnauthorizedAccessException()
    {
        var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        context.Workouts.Add(new Workout { Id = workoutId, UserId = ownerId, Name = "Push Day", Date = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.GetWorkoutById(Guid.NewGuid(), workoutId));
    }

    [Fact]
    public async Task GetWorkoutById_WhenWorkoutDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        var sut = new WorkoutService(CreateContext());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.GetWorkoutById(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteWorkout_WhenOwnedByUser_RemovesWorkout()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        context.Workouts.Add(new Workout { Id = workoutId, UserId = userId, Name = "Leg Day", Date = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);
        await sut.DeleteWorkout(userId, workoutId);

        Assert.Empty(context.Workouts);
    }

    [Fact]
    public async Task DeleteWorkout_WhenNotOwnedByUser_ThrowsUnauthorizedAccessExceptionAndDoesNotDelete()
    {
        var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        context.Workouts.Add(new Workout { Id = workoutId, UserId = ownerId, Name = "Leg Day", Date = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.DeleteWorkout(Guid.NewGuid(), workoutId));
        Assert.Single(context.Workouts);
    }

    [Fact]
    public async Task UpdateWorkout_WhenOwnedByUser_UpdatesNameAndDate()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        context.Workouts.Add(new Workout { Id = workoutId, UserId = userId, Name = "Old Name", Date = new DateTime(2026, 1, 1) });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);
        var newDate = new DateTime(2026, 2, 2);
        var result = await sut.UpdateWorkout(userId, workoutId, new UpdateWorkoutDto("New Name", newDate));

        Assert.Equal("New Name", result.Name);
        Assert.Equal(newDate, result.Date);
    }

    [Fact]
    public async Task UpdateWorkout_WhenDateNotProvided_KeepsExistingDate()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var originalDate = new DateTime(2026, 1, 1);
        context.Workouts.Add(new Workout { Id = workoutId, UserId = userId, Name = "Old Name", Date = originalDate });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);
        var result = await sut.UpdateWorkout(userId, workoutId, new UpdateWorkoutDto("New Name", null));

        Assert.Equal(originalDate, result.Date);
    }

    [Fact]
    public async Task UpdateWorkout_WhenNotOwnedByUser_ThrowsUnauthorizedAccessException()
    {
        var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        context.Workouts.Add(new Workout { Id = workoutId, UserId = ownerId, Name = "Old Name", Date = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var sut = new WorkoutService(context);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.UpdateWorkout(Guid.NewGuid(), workoutId, new UpdateWorkoutDto("New Name", null)));
    }
}
