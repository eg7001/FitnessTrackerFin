namespace FitnessTracker.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
