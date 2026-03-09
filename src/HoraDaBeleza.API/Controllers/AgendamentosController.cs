using HoraDaBeleza.Application.Commands.Agendamentos;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.Agendamentos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Agendamentos de serviços</summary>
[ApiController]
[Route("api/agendamentos")]
[Tags("Agendamentos")]
[Produces("application/json")]
public class AgendamentosController : ApiController
{
    private readonly IMediator _mediator;
    public AgendamentosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar meus agendamentos (cliente autenticado)</summary>
    /// <response code="200">Lista de agendamentos do cliente</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("meus")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MeusAgendamentos()
    {
        var result = await _mediator.Send(new ListarAgendamentosClienteQuery(UsuarioId));
        return Ok(result);
    }

    /// <summary>Listar agenda do salão</summary>
    /// <param name="salaoId">ID do salão</param>
    /// <param name="data">Filtrar por data específica (opcional)</param>
    /// <response code="200">Agendamentos do salão</response>
    [HttpGet("salao/{salaoId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AgendamentosSalao(int salaoId, [FromQuery] DateTime? data)
    {
        var result = await _mediator.Send(new ListarAgendamentosSalaoQuery(salaoId, data));
        return Ok(result);
    }

    /// <summary>Listar agenda do profissional</summary>
    /// <param name="profissionalId">ID do profissional</param>
    /// <param name="data">Filtrar por data específica (opcional)</param>
    /// <response code="200">Agendamentos do profissional</response>
    [HttpGet("profissional/{profissionalId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<AgendamentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AgendamentosProfissional(int profissionalId, [FromQuery] DateTime? data)
    {
        var result = await _mediator.Send(new ListarAgendamentosProfissionalQuery(profissionalId, data));
        return Ok(result);
    }

    /// <summary>Criar novo agendamento</summary>
    /// <remarks>
    /// A API valida automaticamente conflito de horário com outros agendamentos do mesmo profissional.
    /// A duração e o valor são herdados do serviço selecionado.
    /// </remarks>
    /// <response code="201">Agendamento criado</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="422">Horário indisponível ou serviço inativo</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(AgendamentoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Criar([FromBody] CriarAgendamentoRequest request)
    {
        var result = await _mediator.Send(new CriarAgendamentoCommand(
            UsuarioId, request.ProfissionalId, request.ServicoId,
            request.SalaoId, request.DataHora, request.Observacoes));
        return Created($"/api/agendamentos/{result.Id}", result);
    }

    /// <summary>Cancelar agendamento</summary>
    /// <remarks>Pode ser cancelado pelo cliente ou pelo profissional responsável.</remarks>
    /// <response code="204">Agendamento cancelado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Agendamento não encontrado</response>
    /// <response code="422">Agendamento já concluído ou já cancelado</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _mediator.Send(new CancelarAgendamentoCommand(id, UsuarioId));
        return NoContent();
    }

    /// <summary>Atualizar status do agendamento (profissional)</summary>
    /// <remarks>
    /// Status disponíveis: **1**=Pendente, **2**=Confirmado, **3**=Cancelado, **4**=Concluído, **5**=NãoCompareceu
    /// </remarks>
    /// <response code="204">Status atualizado</response>
    /// <response code="401">Não autenticado ou sem permissão</response>
    /// <response code="404">Agendamento não encontrado</response>
    [HttpPatch("{id:int}/status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusAgendamentoRequest request)
    {
        await _mediator.Send(new AtualizarStatusAgendamentoCommand(id, UsuarioId, request.Status));
        return NoContent();
    }
}
