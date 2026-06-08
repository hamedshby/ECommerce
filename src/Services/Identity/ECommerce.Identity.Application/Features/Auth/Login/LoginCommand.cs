using MediatR;
using ECommerce.Identity.Application.Common;

namespace ECommerce.Identity.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid UserId,
    string FullName
);
