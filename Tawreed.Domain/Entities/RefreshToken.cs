using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class RefreshToken : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public User User { get; set; } = null!;
}
