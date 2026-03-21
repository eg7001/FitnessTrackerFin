using FitnessTracker.DTOs.Auth;
using FitnessTracker.DTOs.Refresh;

namespace FitnessTracker.Services.Interfaces
{
    public interface IAuthService
    {
        Task Register(RegisterDto dto);
        Task<object> Login(LoginDto dto);
        Task<object> RefreshToken(TokenRefreshRequestDto dto);
    }
}
