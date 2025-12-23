using FitnessTracker.DTOs.Exercise;
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

        [HttpPost("addWorkout")]
        public async Task<IActionResult> CreateWorkout([FromBody]CreateWorkoutDto dto)
        {
            var workout = await _workoutService.CreateWorkout(GetUserGuid(), dto);
            return Ok(workout);
        }

        [HttpGet("getWorkouts")]
        public async Task<IActionResult> GetWorkouts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var workouts = await _workoutService.GetUserWorkouts(GetUserGuid(),page, pageSize);
            return Ok(workouts);
        }
        [HttpDelete("{workoutExerciseId}/deleteExercise")]
        public async Task<IActionResult> DeleteExercise(int workoutExercsieId) { 
            await _workoutService.DeleteWorkoutExercise(GetUserGuid(),workoutExercsieId;
            return Ok("The Exercise Was Deleted Succesfully");
        }
        

        [HttpPost("{workoutId}/addExercises")] 
        public async Task<IActionResult> AddWorkout([FromRoute]Guid workoutId,[FromBody]AddWorkoutExerciseDto dto)
        {
            try
            {
                await _workoutService.AddExerciseToWorkout(GetUserGuid(), workoutId, dto);
                return Ok("The Exercise Was Addedd succesfully");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{workoutId}/delete")]
        public async Task<IActionResult> DeleteWorkout([FromRoute]Guid workoutId) {
            try
            {
                await _workoutService.DeleteWorkout(GetUserGuid(), workoutId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{workoutId}/update")]
        public async Task<IActionResult> UpdateWorkout([FromRoute] Guid workoutId,UpdateWorkoutDto dto) { 
            await _workoutService.UpdateWorkout(GetUserGuid(), workoutId, dto);
            return Ok("The Workout Updated Succesfully");
        }


    }
}
