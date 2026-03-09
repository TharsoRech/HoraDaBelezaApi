using HoraDaBeleza.Application.Commands.Assinaturas;
using HoraDaBeleza.Application.Commands.Avaliacoes;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.Avaliacoes;
using HoraDaBeleza.Application.Queries.Planos;
using HoraDaBeleza.Application.Queries.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

// ── Avaliações ─────────────────────────────────────────────────────────────
[Route("api/avaliacoes")]
public class AvaliacoesController : ApiController
{
    private readonly IMediator _mediator;
    public AvaliacoesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("salao/{salaoId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> PorSalao(int salaoId)
    {
        var result = await _mediator.Send(new ListarAvaliacoesSalaoQuery(salaoId));
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar([FromBody] CriarAvaliacaoRequest request)
    {
        var result = await _mediator.Send(
            new CriarAvaliacaoCommand(request.AgendamentoId, UsuarioId, request.Nota, request.Comentario));
        return Created("", result);
    }
}

// ── Planos ─────────────────────────────────────────────────────────────────
[Route("api/planos")]
public class PlanosController : ApiController
{
    private readonly IMediator _mediator;
    public PlanosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var result = await _mediator.Send(new ListarPlanosQuery());
        return Ok(result);
    }
}

// ── Assinaturas ────────────────────────────────────────────────────────────
[Route("api/assinaturas")]
public class AssinaturasController : ApiController
{
    private readonly IMediator _mediator;
    public AssinaturasController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Assinar([FromBody] CriarAssinaturaRequest request)
    {
        var result = await _mediator.Send(
            new CriarAssinaturaCommand(request.SalaoId, UsuarioId, request.PlanoId));
        return Created("", result);
    }
}

// ── Notificações ───────────────────────────────────────────────────────────
[Route("api/notificacoes")]
public class NotificacoesController : ApiController
{
    private readonly IMediator _mediator;
    private readonly Application.Interfaces.INotificacaoRepository _repo;

    public NotificacoesController(IMediator mediator,
        Application.Interfaces.INotificacaoRepository repo)
    {
        _mediator = mediator;
        _repo     = repo;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Listar([FromQuery] bool apenasNaoLidas = false)
    {
        var result = await _mediator.Send(new ListarNotificacoesQuery(UsuarioId, apenasNaoLidas));
        return Ok(result);
    }

    [HttpPatch("{id:int}/lida")]
    [Authorize]
    public async Task<IActionResult> MarcarLida(int id)
    {
        await _repo.MarcarComoLidaAsync(id, UsuarioId);
        return NoContent();
    }

    [HttpPatch("lidas")]
    [Authorize]
    public async Task<IActionResult> MarcarTodasLidas()
    {
        await _repo.MarcarTodasComoLidasAsync(UsuarioId);
        return NoContent();
    }
}
