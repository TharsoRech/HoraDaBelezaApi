using HoraDaBeleza.Application.Commands.Users;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.GetProfileQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Authentication and user profile</summary>
[Route("api/auth")]
[Tags("Auth")]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Login — returns a JWT token</summary>
    /// <remarks>Paste the returned token into **Authorize 🔒** as: `Bearer {token}`</remarks>
    /// <response code="200">Token generated</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
        => Ok(await _mediator.Send(new LoginCommand(request.Email, request.Password)));

    /// <summary>Register a new user</summary>
    /// <remarks>Types: **1**=Client, **2**=Professional, **3**=Owner, **4**=Admin</remarks>
    /// <response code="201">User created</response>
    /// <response code="400">Validation error</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(
            new RegisterUserCommand(request.Name, request.Email, request.Password, request.Phone, request.Type));
        return Created($"/api/users/{result.Id}", result);
    }

    /// <summary>Get authenticated user profile</summary>
    /// <response code="200">Profile returned</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Me()
        => Ok(await _mediator.Send(new GetProfileQuery(UserId)));

    /// <summary>Update authenticated user profile</summary>
    /// <response code="200">Profile updated</response>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        => Ok(await _mediator.Send(new UpdateProfileCommand(UserId, request.Name, request.Phone, request.PhotoUrl)));
}
