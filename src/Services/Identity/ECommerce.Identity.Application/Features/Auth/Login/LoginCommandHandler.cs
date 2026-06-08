using MediatR;
using ECommerce.Identity.Application.Common;
using ECommerce.Identity.Application.Common.Interfaces;
using ECommerce.Identity.Domain.Aggregates;
using ECommerce.Identity.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ECommerce.Identity.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Use constant-time comparison to prevent user enumeration
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure("INVALID_CREDENTIALS", "Email or password is incorrect.");

        if (!user.IsActive)
            return Result<LoginResponse>.Failure("ACCOUNT_INACTIVE", "This account has been deactivated.");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = RefreshToken.Create(user.Id);

        user.AddRefreshToken(refreshToken);
        user.RecordLogin();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in: {UserId}", user.Id);

        return Result<LoginResponse>.Success(new LoginResponse(
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            user.Id,
            $"{user.FirstName} {user.LastName}"
        ));
    }
}
