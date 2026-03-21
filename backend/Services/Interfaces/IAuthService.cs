using FitnessTracker.DTOs.Auth;

namespace FitnessTracker.Services.Interfaces
{
    public interface IAuthService
    {
        Task Register(RegisterDto dto);
        Task<string> Login(LoginDto dto);
    }
}
