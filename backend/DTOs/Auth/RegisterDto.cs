using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.DTOs.Auth
{
    public record RegisterDto(
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        string Email,
        [Required]
        [MaxLength(50)]
        string Password);

}
