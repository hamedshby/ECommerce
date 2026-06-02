using ECommerce.SharedKernel.Abstractions;

namespace ECommerce.Identity.Domain.Aggregates;

public sealed class RefreshToken : Entity
{
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public string? RevokedReason { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid UserId { get; private set; }

    // Required by EF Core
    private RefreshToken() { }

    private RefreshToken(string token, DateTime expiresAt, Guid userId)
    {
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
        IsRevoked = false;
    }

    public static RefreshToken Create(Guid userId, int expiryDays = 7)
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes);
        return new RefreshToken(token, DateTime.UtcNow.AddDays(expiryDays), userId);
    }

    public void Revoke(string reason)
    {
        IsRevoked = true;
        RevokedReason = reason;
        RevokedAt = DateTime.UtcNow;
    }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
