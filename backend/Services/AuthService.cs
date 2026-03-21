using FitnessTracker.DTOs.Auth;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracker.Services
{
    public class AuthService : IAuthService
    {
        UserManager<AppUser> _userManager;
        ITokenService _tokenService;

        public AuthService(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task Register(RegisterDto dto)
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new InvalidOperationException(string.Join(", ", errors));
            }
        }

        public async Task<string> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!valid)
                throw new UnauthorizedAccessException("Invalid credentials");

            return _tokenService.CreateToken(user);
        }
    }
}
