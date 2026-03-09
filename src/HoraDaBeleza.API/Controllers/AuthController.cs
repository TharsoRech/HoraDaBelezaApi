using HoraDaBeleza.Application.Commands.Usuarios;
using HoraDaBeleza.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

[Route("api/auth")]
public class AuthController : ApiController
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Login — retorna JWT</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Senha));
        return Ok(result);
    }

    /// <summary>Registrar novo usuário</summary>
    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
    {
        var result = await _mediator.Send(
            new RegistrarUsuarioCommand(request.Nome, request.Email, request.Senha,
                request.Telefone, request.Tipo));
        return Created($"/api/usuarios/{result.Id}", result);
    }

    /// <summary>Perfil do usuário autenticado</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var result = await _mediator.Send(
            new Application.Queries.Usuarios.ObterPerfilQuery(UsuarioId));
        return Ok(result);
    }

    /// <summary>Atualizar perfil</summary>
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> AtualizarPerfil([FromBody] AtualizarPerfilRequest request)
    {
        var result = await _mediator.Send(
            new AtualizarPerfilCommand(UsuarioId, request.Nome, request.Telefone, request.FotoUrl));
        return Ok(result);
    }
}

public record AtualizarPerfilRequest(string Nome, string? Telefone, string? FotoUrl);
