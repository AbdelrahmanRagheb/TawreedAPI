using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class User : BaseSoftDeletableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PreferredLang { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public string? AvatarUrl { get; set; }

    public Buyer? Buyer { get; set; }
    public Supplier? Supplier { get; set; }
    public DeliveryPersonProfile? DeliveryPersonProfile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
}
