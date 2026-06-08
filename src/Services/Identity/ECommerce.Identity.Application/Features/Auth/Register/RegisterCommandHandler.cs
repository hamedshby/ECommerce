using MediatR;
using ECommerce.Identity.Application.Common;
using ECommerce.Identity.Application.Common.Interfaces;
using ECommerce.Identity.Domain.Aggregates;
using ECommerce.Identity.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ECommerce.Identity.Application.Features.Auth.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
            return Result<RegisterResponse>.Failure("EMAIL_TAKEN", "An account with this email already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("New user registered: {UserId} - {Email}", user.Id, user.Email.Value);

        return Result<RegisterResponse>.Success(new RegisterResponse(
            user.Id,
            user.Email.Value,
            $"{user.FirstName} {user.LastName}"
        ));
    }
}
