using FitnessTracker.DTOs.Set;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Controllers
{
    [Route("api/sets")]
    [ApiController]
    [Authorize]
    public class SetsController : ControllerBase
    {
        private readonly ISetService _setService;
        public SetsController(ISetService setService)
        {
            _setService = setService;
        }
        private Guid GetUserGuid() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // ADD set
        [HttpPost("/api/workout-exercises/{workoutExerciseId}/sets")]
        public async Task<IActionResult> AddSet(
            Guid workoutId,
            int workoutExerciseId,
            AddSetDto dto)
        {
            await _setService.AddSetToWorkoutExercise(GetUserGuid(), workoutExerciseId, dto);
            return NoContent();
        }
        [HttpPut("{setId}")]
        public async Task<IActionResult> UpdateSet(
            Guid workoutId,
            int setId,
            UpdateSetDto dto
            )
        {
            await _setService.UpdateSet(GetUserGuid(), setId, dto);
            return NoContent();
        }
        [HttpDelete("{setId}")]
        public async Task<IActionResult> DeleteSet(int setId)
        {
            await _setService.DeleteSet(GetUserGuid(), setId);
            return NoContent();
        }
    }
}
