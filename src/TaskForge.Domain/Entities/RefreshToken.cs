using TaskForge.Domain.Common;

namespace TaskForge.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}