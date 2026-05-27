using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(string Token) : IRequest<Result>;