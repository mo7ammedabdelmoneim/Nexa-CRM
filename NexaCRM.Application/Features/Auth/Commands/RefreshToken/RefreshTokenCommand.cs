using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string Token) : IRequest<Result<LoginResponseDto>>;  



