using FitnessTracker.DTOs.Exercise;
using FitnessTracker.DTOs.QueryObject;
using FitnessTracker.DTOs.Workout;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Controllers
{
    [Route("api/exercises")]
    [ApiController]
    [Authorize]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }
        [HttpGet]
        public async Task<IActionResult> GetExercises([FromQuery]ExerciseQueryDto dto)
        {
            var ex = await _exerciseService.GetExercises(dto);
            return Ok(ex);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExerciseById(int id)
        {
            var exercise = await _exerciseService.GetExerciseById(id);
            return Ok(exercise);
        }

        // Exercises are a shared, global catalog (not owned by any one
        // user), so creating/editing/deleting them is restricted to
        // admins. Everyone authenticated can still browse the catalog via
        // GetExercises/GetExerciseById above to add exercises to their own
        // workouts.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseDto createExerciseDto)
        {
            await _exerciseService.CreateExercise(createExerciseDto);
            return Ok("Exercise created successfully");

        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateExercise([FromRoute]int id,[FromBody]ExerciseDto dto)
        {
            await _exerciseService.UpdateExercise(id, dto);
            return Ok("The Exercise has been updated");
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteExerciseById(int id) {
            await _exerciseService.DeleteExercise(id);
            return NoContent();
        }
        
    }
}
