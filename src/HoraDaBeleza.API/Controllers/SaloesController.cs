using HoraDaBeleza.Application.Commands.Saloes;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.Saloes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

public class SaloesController : ApiController
{
    private readonly IMediator _mediator;
    public SaloesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar salões (público) — filtrar por cidade ou busca</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar([FromQuery] string? cidade, [FromQuery] string? busca)
    {
        var result = await _mediator.Send(new ListarSaloesQuery(cidade, busca));
        return Ok(result);
    }

    /// <summary>Obter salão por ID (público)</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var result = await _mediator.Send(new ObterSalaoQuery(id));
        return Ok(result);
    }

    /// <summary>Listar meus salões (proprietário)</summary>
    [HttpGet("meus")]
    [Authorize]
    public async Task<IActionResult> MeusSaloes()
    {
        var result = await _mediator.Send(new ListarSaloesPorProprietarioQuery(UsuarioId));
        return Ok(result);
    }

    /// <summary>Criar salão</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar([FromBody] CriarSalaoRequest request)
    {
        var result = await _mediator.Send(new CriarSalaoCommand(
            UsuarioId, request.Nome, request.Descricao, request.Endereco,
            request.Cidade, request.Estado, request.Cep,
            request.Latitude, request.Longitude, request.Telefone,
            request.Email, request.HorarioFuncionamento));
        return Created($"/api/saloes/{result.Id}", result);
    }

    /// <summary>Atualizar salão</summary>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarSalaoRequest request)
    {
        var result = await _mediator.Send(new AtualizarSalaoCommand(
            id, UsuarioId, request.Nome, request.Descricao, request.Endereco,
            request.Cidade, request.Estado, request.Cep,
            request.Latitude, request.Longitude, request.Telefone,
            request.Email, request.HorarioFuncionamento));
        return Ok(result);
    }

    /// <summary>Deletar (desativar) salão</summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Deletar(int id)
    {
        await _mediator.Send(new DeletarSalaoCommand(id, UsuarioId));
        return NoContent();
    }
}
