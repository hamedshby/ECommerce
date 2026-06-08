using ECommerce.Identity.Domain.Aggregates;

namespace ECommerce.Identity.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    bool ValidateToken(string token, out Guid userId);
}
