using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Controllers
{
    [ApiController]
    [Route("api/workouts")]
    [Authorize]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;
        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }
        private Guid GetUserGuid() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost]
        public async Task<IActionResult> CreateWorkout([FromBody]CreateWorkoutDto dto)
        {
            var workout = await _workoutService.CreateWorkout(GetUserGuid(), dto);
            return Ok(workout);
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkouts()
        {
            var workouts = await _workoutService.GetUserWorkouts(GetUserGuid());
            if (workouts == null)
                return BadRequest("There are no workouts for this user"); 
            else
                return Ok(workouts);
        }

        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> AddWorkout([FromRoute]Guid workoutId,[FromBody]AddExerciseDto dto)
        {
            await _workoutService.AddExerciseToWorkout(workoutId, dto);
            return Ok("The Exercise was added succesfully");
        }
    }
}
