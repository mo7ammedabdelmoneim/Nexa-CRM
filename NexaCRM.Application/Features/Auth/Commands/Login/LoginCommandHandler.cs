using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Repositories;
using DomainRefreshToken = NexaCRM.Domain.Aggregates.Users.RefreshToken;

namespace NexaCRM.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            command.Email, command.TenantId, cancellationToken);

        if (user is null)
            return Result<LoginResponseDto>.Failure("Invalid email or password.");

        if (!user.IsActive)
            return Result<LoginResponseDto>.Failure("Account is deactivated.");

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Failure("Invalid email or password.");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = _jwtService.HashToken(refreshToken);

        var refreshTokenEntity = DomainRefreshToken.Create(user.Id, refreshTokenHash);
        user.RecordLogin();

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResponseDto>.Success(new LoginResponseDto(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15),
            new UserDto(user.Id, user.Email, user.FullName, user.Role, user.TenantId)));
    }
}