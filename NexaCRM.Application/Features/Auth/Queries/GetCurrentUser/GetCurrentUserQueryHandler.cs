using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
        => _userRepository = userRepository;

    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            query.UserId, cancellationToken);

        if (user is null)
            return Result<UserDto>.Failure("User not found.");

        return Result<UserDto>.Success(
            new UserDto(user.Id, user.Email, user.FullName, user.Role, user.TenantId));
    }
}