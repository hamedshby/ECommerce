using ECommerce.SharedKernel.Abstractions;
using ECommerce.Identity.Domain.Events;
using ECommerce.Identity.Domain.Exceptions;
using ECommerce.Identity.Domain.ValueObjects;

namespace ECommerce.Identity.Domain.Aggregates;

public sealed class User : Entity
{
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    // Required by EF Core
    private User() { }

    private User(Email email, string passwordHash, string firstName, string lastName)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;

        RaiseDomainEvent(new UserRegisteredEvent(Id, email.Value, firstName, lastName));
    }

    public static User Create(string email, string passwordHash, string firstName, string lastName)
    {
        var emailVo = Email.Create(email);
        return new User(emailVo, passwordHash, firstName, lastName);
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        RaiseDomainEvent(new UserLoggedInEvent(Id, Email.Value));
    }

    public void AddRefreshToken(RefreshToken token)
    {
        RevokeOldRefreshTokens();
        _refreshTokens.Add(token);
    }

    public RefreshToken? GetActiveRefreshToken(string token)
    {
        return _refreshTokens.FirstOrDefault(t => t.Token == token && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);
    }

    public void RevokeRefreshToken(string token, string reason)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token)
            ?? throw new IdentityDomainException("INVALID_TOKEN", "Refresh token not found.");

        refreshToken.Revoke(reason);
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new IdentityDomainException("ALREADY_INACTIVE", "User is already inactive.");

        IsActive = false;
    }

    private void RevokeOldRefreshTokens()
    {
        var activeTokens = _refreshTokens.Where(t => !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow).ToList();

        // Keep max 5 active tokens (multi-device support)
        if (activeTokens.Count >= 5)
            activeTokens.First().Revoke("Exceeded max active sessions.");
    }
}
