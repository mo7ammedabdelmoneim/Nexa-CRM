using NexaCRM.Application.DTOs;
using NexaCRM.Domain.Aggregates.Users;

namespace NexaCRM.Application.Contracts;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
    UserDto? ValidateAccessToken(string token);
}