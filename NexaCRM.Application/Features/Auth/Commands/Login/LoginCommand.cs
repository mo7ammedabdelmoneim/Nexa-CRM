using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;

namespace NexaCRM.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    Guid TenantId) : IRequest<Result<LoginResponseDto>>;