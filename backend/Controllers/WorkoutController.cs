using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.Set;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.DTOs.WorkoutExercise;
using FitnessTracker.Models;
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

        //Create Workout
        [HttpPost]
        public async Task<IActionResult> CreateWorkout([FromBody]CreateWorkoutDto dto)
        {
            var workout = await _workoutService.CreateWorkout(GetUserGuid(), dto);
            return Ok(workout);
        }
        //Get Workouts
        [HttpGet]
        public async Task<IActionResult> GetWorkouts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var workouts = await _workoutService.GetUserWorkouts(GetUserGuid(),page, pageSize);
            return Ok(workouts);
        }
        [HttpGet("{workoutId}")]
        public async Task<IActionResult> GetWorkoutById(Guid workoutId)
        {
            try
            {
                var workout = await _workoutService.GetWorkoutById(
                    GetUserGuid(),
                    workoutId
                );

                return Ok(workout);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }
        // UPDATE workout
        [HttpPut("{workoutId}")]
        public async Task<IActionResult> UpdateWorkout([FromRoute] Guid workoutId, UpdateWorkoutDto dto)
        {
            await _workoutService.UpdateWorkout(GetUserGuid(), workoutId, dto);
            return Ok("The Workout Updated Succesfully");
        }
        
        //Delete Workout
        [HttpDelete("{workoutId}")]
        public async Task<IActionResult> DeleteWorkout([FromRoute] Guid workoutId)
        {
            await _workoutService.DeleteWorkout(GetUserGuid(), workoutId);
            return NoContent();
        }

        // ADD exercise
        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> AddExercise(
            Guid workoutId,
            AddWorkoutExerciseDto dto)
        {
            await _workoutService.AddExerciseToWorkout(GetUserGuid(), workoutId, dto);
            return NoContent();
        }
        // DELETE exercise
        [HttpDelete("{workoutId}/exercises/{workoutExerciseId}")]
        public async Task<IActionResult> DeleteExercise(
            Guid workoutId,
            int workoutExerciseId)
        {
            await _workoutService.DeleteWorkoutExercise(GetUserGuid(), workoutExerciseId);
            return NoContent();
        }


        // ADD set
        [HttpPost("sets")]
        public async Task<IActionResult> AddSet(
            Guid workoutId,
            int workoutExerciseId,
            AddSetDto dto)
        {
            await _workoutService.AddSetToWorkoutExercise(GetUserGuid(), workoutExerciseId, dto);
            return NoContent();
        }
        [HttpPost("sets/{setId}")]
        public async Task<IActionResult> UpdateSet(
            Guid workoutId,
            int setId,
            UpdateSetDto dto
            )
        {
            await _workoutService.UpdateSet(GetUserGuid(), setId, dto);
            return NoContent();
        }
        [HttpDelete("sets/{setId}")]
        public async Task<IActionResult> DeleteSet(int setId)
        {
            await  _workoutService.DeleteSet(GetUserGuid(), setId); 
            return NoContent();
        }
    }
}
