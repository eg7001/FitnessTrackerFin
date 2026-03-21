using FitnessTracker.DTOs.Auth;
using FitnessTracker.DTOs.Refresh;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _authService.Register(dto);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.Login(dto);
        return Ok(token);
    }
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(TokenRefreshRequestDto dto)
    {
        var result = await _authService.RefreshToken(dto);
        return Ok(result);
    }
}
