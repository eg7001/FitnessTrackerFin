using FitnessTracker.DTOs.Exercise;
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
        public async Task<IActionResult> GetExerciseById(int id)
        {
            var exercise = await _exerciseService.GetExerciseById(id);
            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseDto createExerciseDto)
        {
            await _exerciseService.CreateExercise(createExerciseDto);
            return Ok("Exercise created successfully");
            
        }
        
    }
}
