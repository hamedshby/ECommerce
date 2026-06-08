using MediatR;
using ECommerce.Identity.Application.Common;

namespace ECommerce.Identity.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<RegisterResponse>>;

public sealed record RegisterResponse(
    Guid UserId,
    string Email,
    string FullName
);
