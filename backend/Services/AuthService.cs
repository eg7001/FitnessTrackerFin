using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Auth;
using FitnessTracker.DTOs.Refresh;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Services
{
    public class AuthService : IAuthService
    {
        UserManager<AppUser> _userManager;
        ITokenService _tokenService;
        ApplicationDbContext _context;

        public AuthService(UserManager<AppUser> userManager, ITokenService tokenService,ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task Register(RegisterDto dto)
        {
            // Whoever signs up first becomes the catalog admin - there's no
            // admin UI yet, so this is the only way anyone ever gets the
            // "Admin" role. Everyone after the first user is a normal user.
            // Synchronous by design: EF's async LINQ operators require an
            // async query provider, which a plain mocked/in-memory
            // IQueryable in tests won't have. A one-off check on a rare
            // operation like registration doesn't need to be async here.
            var isFirstUser = !_userManager.Users.Any();

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

            if (isFirstUser)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
        public async Task<object> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid)
                throw new UnauthorizedAccessException("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new
            {
                accessToken,
                refreshToken
            };
        }
        public async Task<object> RefreshToken(TokenRefreshRequestDto dto)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var roles = await _userManager.GetRolesAsync(storedToken.User);
            var newAccessToken = _tokenService.CreateToken(storedToken.User, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // revoke old token
            storedToken.IsRevoked = true;

            // create new one
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = storedToken.UserId
            });

            await _context.SaveChangesAsync();

            return new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            };
        }
    }
}
