using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommandHandler
    : IRequestHandler<RevokeTokenCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RevokeTokenCommand command,
        CancellationToken cancellationToken)
    {
        var tokenHash = _jwtService.HashToken(command.Token);

        var refreshToken = await _refreshTokenRepository
            .GetByTokenHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null)
            return Result.Failure("Token not found.");

        refreshToken.Revoke();

        _refreshTokenRepository.Update(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}