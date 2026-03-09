using HoraDaBeleza.Application.Commands.Assinaturas;
using HoraDaBeleza.Application.Commands.Avaliacoes;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Application.Queries.Avaliacoes;
using HoraDaBeleza.Application.Queries.Planos;
using HoraDaBeleza.Application.Queries.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

// ── Avaliações ─────────────────────────────────────────────────────────────

/// <summary>Avaliações de salões e profissionais</summary>
[ApiController]
[Route("api/avaliacoes")]
[Tags("Avaliações")]
[Produces("application/json")]
public class AvaliacoesController : ApiController
{
    private readonly IMediator _mediator;
    public AvaliacoesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar avaliações de um salão (público)</summary>
    /// <response code="200">Lista de avaliações</response>
    [HttpGet("salao/{salaoId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<AvaliacaoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PorSalao(int salaoId)
    {
        var result = await _mediator.Send(new ListarAvaliacoesSalaoQuery(salaoId));
        return Ok(result);
    }

    /// <summary>Criar avaliação para um agendamento concluído</summary>
    /// <remarks>
    /// - Apenas o cliente do agendamento pode avaliar.
    /// - O agendamento deve estar com status **Concluído** (4).
    /// - Cada agendamento só pode ser avaliado uma vez.
    /// - A nota média do profissional é recalculada automaticamente.
    /// </remarks>
    /// <response code="201">Avaliação criada</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="422">Agendamento não concluído ou já avaliado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(AvaliacaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Criar([FromBody] CriarAvaliacaoRequest request)
    {
        var result = await _mediator.Send(
            new CriarAvaliacaoCommand(request.AgendamentoId, UsuarioId, request.Nota, request.Comentario));
        return Created("", result);
    }
}

// ── Planos ─────────────────────────────────────────────────────────────────

/// <summary>Planos de assinatura para salões</summary>
[ApiController]
[Route("api/planos")]
[Tags("Planos & Assinaturas")]
[Produces("application/json")]
public class PlanosController : ApiController
{
    private readonly IMediator _mediator;
    public PlanosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar planos disponíveis (público)</summary>
    /// <response code="200">Lista de planos ativos</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PlanoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var result = await _mediator.Send(new ListarPlanosQuery());
        return Ok(result);
    }
}

// ── Assinaturas ────────────────────────────────────────────────────────────

/// <summary>Assinaturas de planos para salões</summary>
[ApiController]
[Route("api/assinaturas")]
[Tags("Planos & Assinaturas")]
[Produces("application/json")]
public class AssinaturasController : ApiController
{
    private readonly IMediator _mediator;
    public AssinaturasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Assinar um plano para o salão</summary>
    /// <remarks>
    /// Se o salão já possui uma assinatura ativa, ela será cancelada automaticamente
    /// e a nova assinatura será criada.
    /// </remarks>
    /// <response code="201">Assinatura criada</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não é o proprietário do salão</response>
    /// <response code="404">Salão ou plano não encontrado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(AssinaturaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assinar([FromBody] CriarAssinaturaRequest request)
    {
        var result = await _mediator.Send(
            new CriarAssinaturaCommand(request.SalaoId, UsuarioId, request.PlanoId));
        return Created("", result);
    }
}

// ── Notificações ───────────────────────────────────────────────────────────

/// <summary>Notificações do usuário autenticado</summary>
[ApiController]
[Route("api/notificacoes")]
[Tags("Notificações")]
[Produces("application/json")]
public class NotificacoesController : ApiController
{
    private readonly IMediator _mediator;
    private readonly INotificacaoRepository _repo;

    public NotificacoesController(IMediator mediator, INotificacaoRepository repo)
    {
        _mediator = mediator;
        _repo     = repo;
    }

    /// <summary>Listar notificações do usuário autenticado</summary>
    /// <param name="apenasNaoLidas">Se true, retorna apenas as não lidas</param>
    /// <response code="200">Lista de notificações</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<NotificacaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Listar([FromQuery] bool apenasNaoLidas = false)
    {
        var result = await _mediator.Send(new ListarNotificacoesQuery(UsuarioId, apenasNaoLidas));
        return Ok(result);
    }

    /// <summary>Marcar notificação como lida</summary>
    /// <response code="204">Marcada como lida</response>
    /// <response code="401">Não autenticado</response>
    [HttpPatch("{id:int}/lida")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarcarLida(int id)
    {
        await _repo.MarcarComoLidaAsync(id, UsuarioId);
        return NoContent();
    }

    /// <summary>Marcar todas as notificações como lidas</summary>
    /// <response code="204">Todas marcadas como lidas</response>
    /// <response code="401">Não autenticado</response>
    [HttpPatch("lidas")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarcarTodasLidas()
    {
        await _repo.MarcarTodasComoLidasAsync(UsuarioId);
        return NoContent();
    }
}
