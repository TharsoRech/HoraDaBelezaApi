using HoraDaBeleza.Application.Commands.Agendamentos;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.Agendamentos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

public class AgendamentosController : ApiController
{
    private readonly IMediator _mediator;
    public AgendamentosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Meus agendamentos (cliente)</summary>
    [HttpGet("meus")]
    [Authorize]
    public async Task<IActionResult> MeusAgendamentos()
    {
        var result = await _mediator.Send(new ListarAgendamentosClienteQuery(UsuarioId));
        return Ok(result);
    }

    /// <summary>Agendamentos do salão (proprietário)</summary>
    [HttpGet("salao/{salaoId:int}")]
    [Authorize]
    public async Task<IActionResult> AgendamentosSalao(int salaoId, [FromQuery] DateTime? data)
    {
        var result = await _mediator.Send(new ListarAgendamentosSalaoQuery(salaoId, data));
        return Ok(result);
    }

    /// <summary>Agendamentos do profissional</summary>
    [HttpGet("profissional/{profissionalId:int}")]
    [Authorize]
    public async Task<IActionResult> AgendamentosProfissional(int profissionalId, [FromQuery] DateTime? data)
    {
        var result = await _mediator.Send(new ListarAgendamentosProfissionalQuery(profissionalId, data));
        return Ok(result);
    }

    /// <summary>Criar agendamento</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar([FromBody] CriarAgendamentoRequest request)
    {
        var result = await _mediator.Send(new CriarAgendamentoCommand(
            UsuarioId, request.ProfissionalId, request.ServicoId,
            request.SalaoId, request.DataHora, request.Observacoes));
        return Created($"/api/agendamentos/{result.Id}", result);
    }

    /// <summary>Cancelar agendamento</summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _mediator.Send(new CancelarAgendamentoCommand(id, UsuarioId));
        return NoContent();
    }

    /// <summary>Atualizar status (profissional/proprietário)</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusAgendamentoRequest request)
    {
        await _mediator.Send(new AtualizarStatusAgendamentoCommand(id, UsuarioId, request.Status));
        return NoContent();
    }
}
