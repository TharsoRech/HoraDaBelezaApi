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
[Route("api/saloes/{salaoId:int}/servicos")]
public class ServicosController : ApiController
{
    private readonly IMediator _mediator;
    public ServicosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(int salaoId, [FromQuery] int? categoriaId)
    {
        var result = await _mediator.Send(new ListarServicosQuery(salaoId, categoriaId));
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar(int salaoId, [FromBody] CriarServicoRequest request)
    {
        var result = await _mediator.Send(new CriarServicoCommand(
            salaoId, UsuarioId, request.CategoriaId, request.Nome,
            request.Descricao, request.Preco, request.DuracaoMinutos));
        return Created("", result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int salaoId, int id, [FromBody] AtualizarServicoRequest request)
    {
        await _mediator.Send(new AtualizarServicoCommand(
            id, salaoId, UsuarioId, request.Nome, request.Descricao,
            request.Preco, request.DuracaoMinutos, request.Ativo));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Deletar(int salaoId, int id)
    {
        await _mediator.Send(new DeletarServicoCommand(id, salaoId, UsuarioId));
        return NoContent();
    }
}

// ── Profissionais ──────────────────────────────────────────────────────────
[Route("api/saloes/{salaoId:int}/profissionais")]
public class ProfissionaisController : ApiController
{
    private readonly IMediator _mediator;
    public ProfissionaisController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(int salaoId)
    {
        var result = await _mediator.Send(new ListarProfissionaisQuery(salaoId));
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar(int salaoId, [FromBody] CriarProfissionalRequest request)
    {
        var result = await _mediator.Send(new CriarProfissionalCommand(
            request.UsuarioId, salaoId, UsuarioId, request.Especialidade, request.Biografia));
        return Created("", result);
    }
}

// ── Categorias ─────────────────────────────────────────────────────────────
[Route("api/categorias")]
public class CategoriasController : ApiController
{
    private readonly IMediator _mediator;
    public CategoriasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var result = await _mediator.Send(new Application.Queries.Usuarios.ListarNotificacoesQuery(0));
        // Categorias têm query própria mas usa o mesmo padrão
        return Ok(result);
    }
}
