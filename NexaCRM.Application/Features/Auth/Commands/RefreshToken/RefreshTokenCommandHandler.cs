using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Repositories;
using DomainRefreshToken = NexaCRM.Domain.Aggregates.Users.RefreshToken;

namespace NexaCRM.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var tokenHash = _jwtService.HashToken(command.Token);

        var refreshToken = await _refreshTokenRepository
            .GetByTokenHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null || !refreshToken.IsValid())
            return Result<LoginResponseDto>.Failure("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(
            refreshToken.UserId, cancellationToken);

        if (user is null || !user.IsActive)
            return Result<LoginResponseDto>.Failure("User not found or inactive.");

        refreshToken.Revoke();

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var newRefreshTokenHash = _jwtService.HashToken(newRefreshToken);

        var newRefreshTokenEntity = DomainRefreshToken.Create(
            user.Id, newRefreshTokenHash);

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
        _refreshTokenRepository.Update(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResponseDto>.Success(new LoginResponseDto(
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(15),
            new UserDto(user.Id, user.Email, user.FullName, user.Role, user.TenantId)));
    }
}