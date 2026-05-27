using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Users;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var exists = await _userRepository.ExistsByEmailAsync(
            command.Email, command.TenantId, cancellationToken);

        if (exists)
            return Result<UserDto>.Failure("Email already registered.");

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = User.Create(
            command.Email,
            passwordHash,
            command.FullName,
            command.TenantId,
            command.Role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserDto>.Success(
            new UserDto(user.Id, user.Email, user.FullName, user.Role, user.TenantId));
    }
}