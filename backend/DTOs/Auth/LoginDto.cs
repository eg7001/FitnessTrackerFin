using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.DTOs.Auth
{
    public record LoginDto(
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        string Email,
        [Required]
        [MaxLength(50)]
        string Password);

}
