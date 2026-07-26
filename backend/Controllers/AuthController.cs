using FitnessTracker.DTOs.Auth;
using FitnessTracker.DTOs.Refresh;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FitnessTracker.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string RefreshCookieName = "refreshToken";

    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _authService.Register(dto);
        return Ok();
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);
        return Ok(IssueAccessTokenAndSetCookie(result));
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await _authService.RefreshToken(new TokenRefreshRequestDto { RefreshToken = refreshToken });
        return Ok(IssueAccessTokenAndSetCookie(result));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.Logout(refreshToken);
        }

        Response.Cookies.Delete(RefreshCookieName, new CookieOptions { Path = "/api/auth" });
        return NoContent();
    }

    // The refresh token never reaches the frontend's JS - it's set as an
    // httpOnly cookie so an XSS bug can't exfiltrate it. Only the
    // short-lived access token goes in the response body.
    private object IssueAccessTokenAndSetCookie(object tokenResult)
    {
        var accessToken = (string)tokenResult.GetType().GetProperty("accessToken")!.GetValue(tokenResult)!;
        var refreshToken = (string)tokenResult.GetType().GetProperty("refreshToken")!.GetValue(tokenResult)!;

        Response.Cookies.Append(RefreshCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            // Match the scheme this request actually arrived on. Neither
            // the dev proxy nor the Docker/nginx setup terminates TLS in
            // front of the frontend, so hardcoding Secure=true here would
            // make browsers silently refuse to store the cookie in both.
            // If the deployment gains real HTTPS in front of the
            // frontend, this starts marking the cookie Secure automatically.
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return new { accessToken };
    }
}
