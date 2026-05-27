using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaCRM.Application.Contracts;
using NexaCRM.Application.Features.Auth.Commands.Login;
using NexaCRM.Application.Features.Auth.Commands.RefreshToken;
using NexaCRM.Application.Features.Auth.Commands.Register;
using NexaCRM.Application.Features.Auth.Commands.RevokeToken;
using NexaCRM.Application.Features.Auth.Queries.GetCurrentUser;

namespace NexaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RegisterCommand(
            request.Email,
            request.Password,
            request.FullName,
            request.TenantId,
            request.Role ?? "SalesRep"), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LoginCommand(
            request.Email,
            request.Password,
            request.TenantId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { error = result.Error });
    }

    // POST api/auth/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RefreshTokenCommand(request.Token), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { error = result.Error });
    }

    // POST api/auth/revoke
    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(
        [FromBody] RevokeTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RevokeTokenCommand(request.Token), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    // GET api/auth/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCurrentUserQuery(_currentUserService.UserId),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }
}

public record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    Guid TenantId,
    string? Role = null);

public record LoginRequest(
    string Email,
    string Password,
    Guid TenantId);

public record RefreshTokenRequest(string Token);
public record RevokeTokenRequest(string Token);