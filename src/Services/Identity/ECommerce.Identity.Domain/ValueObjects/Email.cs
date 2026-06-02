using ECommerce.Identity.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ECommerce.Identity.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new IdentityDomainException("INVALID_EMAIL", "Email cannot be empty.");

        var normalized = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new IdentityDomainException("INVALID_EMAIL", $"'{email}' is not a valid email address.");

        return new Email(normalized);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) =>
        obj is Email other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Email email) => email.Value;
}
