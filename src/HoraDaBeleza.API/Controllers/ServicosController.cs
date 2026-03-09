using HoraDaBeleza.Application.Commands.Profissionais;
using HoraDaBeleza.Application.Commands.Servicos;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.Profissionais;
using HoraDaBeleza.Application.Queries.Servicos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

// ── Serviços ───────────────────────────────────────────────────────────────

/// <summary>Serviços oferecidos por um salão</summary>
[ApiController]
[Route("api/saloes/{salaoId:int}/servicos")]
[Tags("Serviços")]
[Produces("application/json")]
public class ServicosController : ApiController
{
    private readonly IMediator _mediator;
    public ServicosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar serviços do salão (público)</summary>
    /// <param name="salaoId">ID do salão</param>
    /// <param name="categoriaId">Filtrar por categoria (opcional)</param>
    /// <response code="200">Lista de serviços</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ServicoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(int salaoId, [FromQuery] int? categoriaId)
    {
        var result = await _mediator.Send(new ListarServicosQuery(salaoId, categoriaId));
        return Ok(result);
    }

    /// <summary>Criar serviço no salão (somente proprietário)</summary>
    /// <response code="201">Serviço criado</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não é o proprietário do salão</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Criar(int salaoId, [FromBody] CriarServicoRequest request)
    {
        var result = await _mediator.Send(new CriarServicoCommand(
            salaoId, UsuarioId, request.CategoriaId, request.Nome,
            request.Descricao, request.Preco, request.DuracaoMinutos));
        return Created("", result);
    }

    /// <summary>Atualizar serviço (somente proprietário)</summary>
    /// <response code="204">Serviço atualizado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Serviço não encontrado</response>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int salaoId, int id, [FromBody] AtualizarServicoRequest request)
    {
        await _mediator.Send(new AtualizarServicoCommand(
            id, salaoId, UsuarioId, request.Nome, request.Descricao,
            request.Preco, request.DuracaoMinutos, request.Ativo));
        return NoContent();
    }

    /// <summary>Remover serviço (somente proprietário)</summary>
    /// <response code="204">Serviço removido</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Serviço não encontrado</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int salaoId, int id)
    {
        await _mediator.Send(new DeletarServicoCommand(id, salaoId, UsuarioId));
        return NoContent();
    }
}

// ── Profissionais ──────────────────────────────────────────────────────────

/// <summary>Profissionais vinculados a um salão</summary>
[ApiController]
[Route("api/saloes/{salaoId:int}/profissionais")]
[Tags("Profissionais")]
[Produces("application/json")]
public class ProfissionaisController : ApiController
{
    private readonly IMediator _mediator;
    public ProfissionaisController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar profissionais do salão (público)</summary>
    /// <response code="200">Lista de profissionais</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProfissionalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(int salaoId)
    {
        var result = await _mediator.Send(new ListarProfissionaisQuery(salaoId));
        return Ok(result);
    }

    /// <summary>Vincular profissional ao salão (somente proprietário)</summary>
    /// <remarks>O usuário vinculado deve ter tipo **Profissional** (tipo = 2).</remarks>
    /// <response code="201">Profissional vinculado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Usuário ou salão não encontrado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProfissionalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Criar(int salaoId, [FromBody] CriarProfissionalRequest request)
    {
        var result = await _mediator.Send(new CriarProfissionalCommand(
            request.UsuarioId, salaoId, UsuarioId, request.Especialidade, request.Biografia));
        return Created("", result);
    }
}
