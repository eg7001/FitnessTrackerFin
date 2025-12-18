using FitnessTracker.Models;

namespace FitnessTracker.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
