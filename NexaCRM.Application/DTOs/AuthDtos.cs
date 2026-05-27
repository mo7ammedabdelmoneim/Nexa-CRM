namespace NexaCRM.Application.DTOs;

public record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    Guid TenantId);