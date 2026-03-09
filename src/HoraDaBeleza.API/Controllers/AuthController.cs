using HoraDaBeleza.Application.Commands.Usuarios;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Autenticação e gerenciamento de perfil</summary>
[ApiController]
[Route("api/auth")]
[Tags("Auth")]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Login — retorna token JWT</summary>
    /// <remarks>
    /// Exemplo de request:
    /// ```json
    /// { "email": "usuario@email.com", "senha": "Senha@123" }
    /// ```
    /// Copie o token retornado e clique em **Authorize 🔒** no topo da página.
    /// </remarks>
    /// <response code="200">Token JWT gerado com sucesso</response>
    /// <response code="401">Email ou senha inválidos</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Senha));
        return Ok(result);
    }

    /// <summary>Registrar novo usuário</summary>
    /// <remarks>
    /// Tipos disponíveis: **1** = Cliente, **2** = Profissional, **3** = Proprietario, **4** = Admin
    /// </remarks>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="422">Email já cadastrado</response>
    [HttpPost("registrar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
    {
        var result = await _mediator.Send(
            new RegistrarUsuarioCommand(request.Nome, request.Email, request.Senha,
                request.Telefone, request.Tipo));
        return Created($"/api/usuarios/{result.Id}", result);
    }

    /// <summary>Perfil do usuário autenticado</summary>
    /// <response code="200">Perfil retornado com sucesso</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me()
    {
        var result = await _mediator.Send(new ObterPerfilQuery(UsuarioId));
        return Ok(result);
    }

    /// <summary>Atualizar perfil do usuário autenticado</summary>
    /// <response code="200">Perfil atualizado</response>
    /// <response code="401">Não autenticado</response>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AtualizarPerfil([FromBody] AtualizarPerfilRequest request)
    {
        var result = await _mediator.Send(
            new AtualizarPerfilCommand(UsuarioId, request.Nome, request.Telefone, request.FotoUrl));
        return Ok(result);
    }
}

public record AtualizarPerfilRequest(string Nome, string? Telefone, string? FotoUrl);
