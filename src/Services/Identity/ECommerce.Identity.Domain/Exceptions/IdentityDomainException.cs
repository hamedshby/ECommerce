using ECommerce.SharedKernel.Exceptions;

namespace ECommerce.Identity.Domain.Exceptions;

public sealed class IdentityDomainException : DomainException
{
    public IdentityDomainException(string code, string message)
        : base(code, message) { }
}
