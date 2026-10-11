namespace Payroll.Infrastructure.Identity.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByHash { get; set; }

        public bool IsActive(DateTime utcNow) => RevokedAt is null && ExpiresAt > utcNow;
    }
}
