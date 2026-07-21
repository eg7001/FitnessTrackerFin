using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.QueryObject;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.Models;
using FitnessTracker.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitnessTracker.Tests.Services;

public class ExerciseServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateExercise_AddsExerciseToDatabase()
    {
        var context = CreateContext();
        var sut = new ExerciseService(context);

        await sut.CreateExercise(new CreateExerciseDto("Deadlift", "Back", false));

        var stored = await context.Exercises.SingleAsync();
        Assert.Equal("Deadlift", stored.Name);
        Assert.Equal("Back", stored.MuscleGroup);
        Assert.False(stored.IsBodyweight);
    }

    [Fact]
    public async Task GetExerciseById_WhenExists_ReturnsDto()
    {
        var context = CreateContext();
        var exercise = new Exercise { Name = "Pull-up", MuscleGroup = "Back", IsBodyweight = true };
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var sut = new ExerciseService(context);
        var result = await sut.GetExerciseById(exercise.Id);

        Assert.Equal("Pull-up", result.Name);
        Assert.True(result.IsBodyweight);
    }

    [Fact]
    public async Task GetExerciseById_WhenNotFound_ThrowsKeyNotFoundException()
    {
        var sut = new ExerciseService(CreateContext());

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetExerciseById(999));
    }

    [Fact]
    public async Task GetExercises_FiltersBySearchTerm()
    {
        var context = CreateContext();
        context.Exercises.AddRange(
            new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false },
            new Exercise { Name = "Squat", MuscleGroup = "Legs", IsBodyweight = false });
        await context.SaveChangesAsync();

        var sut = new ExerciseService(context);
        var result = await sut.GetExercises(new ExerciseQueryDto { Search = "Bench" });

        var match = Assert.Single(result);
        Assert.Equal("Bench Press", match.Name);
    }

    [Fact]
    public async Task GetExercises_FiltersByMuscleGroup()
    {
        var context = CreateContext();
        context.Exercises.AddRange(
            new Exercise { Name = "Bench Press", MuscleGroup = "Chest", IsBodyweight = false },
            new Exercise { Name = "Squat", MuscleGroup = "Legs", IsBodyweight = false });
        await context.SaveChangesAsync();

        var sut = new ExerciseService(context);
        var result = await sut.GetExercises(new ExerciseQueryDto { MuscleGroup = "Legs" });

        var match = Assert.Single(result);
        Assert.Equal("Squat", match.Name);
    }

    [Fact]
    public async Task GetExercises_RespectsPagination()
    {
        var context = CreateContext();
        for (var i = 0; i < 5; i++)
        {
            context.Exercises.Add(new Exercise { Name = $"Exercise {i}", MuscleGroup = "Group", IsBodyweight = false });
        }
        await context.SaveChangesAsync();

        var sut = new ExerciseService(context);
        var result = await sut.GetExercises(new ExerciseQueryDto { Page = 1, PageSize = 2 });

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DeleteExercise_WhenExists_RemovesIt()
    {
        var context = CreateContext();
        var exercise = new Exercise { Name = "Lunge", MuscleGroup = "Legs", IsBodyweight = true };
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var sut = new ExerciseService(context);
        await sut.DeleteExercise(exercise.Id);

        Assert.Empty(context.Exercises);
    }

    [Fact]
    public async Task DeleteExercise_WhenNotFound_ThrowsKeyNotFoundException()
    {
        var sut = new ExerciseService(CreateContext());

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.DeleteExercise(999));
    }

    [Fact]
    public async Task UpdateExercise_WhenExists_UpdatesFields()
    {
        var context = CreateContext();
        var exercise = new Exercise { Name = "Old Name", MuscleGroup = "Old Group", IsBodyweight = false };
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var sut = new ExerciseService(context);
        var result = await sut.UpdateExercise(exercise.Id, new ExerciseDto("New Name", "New Group", true));

        Assert.Equal("New Name", result.Name);
        Assert.Equal("New Group", result.MuscleGroup);
        Assert.True(result.IsBodyweight);
    }

    [Fact]
    public async Task UpdateExercise_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Now consistent with DeleteExercise/GetExerciseById: the API
        // returns a proper 404 instead of a generic 500 for this case.
        var sut = new ExerciseService(CreateContext());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => sut.UpdateExercise(999, new ExerciseDto("Name", null, false)));
    }
}
