using HoraDaBeleza.Application.Commands.Saloes;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Queries.Saloes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoraDaBeleza.API.Controllers;

/// <summary>Gerenciamento de salões de beleza</summary>
[ApiController]
[Route("api/saloes")]
[Tags("Salões")]
[Produces("application/json")]
public class SaloesController : ApiController
{
    private readonly IMediator _mediator;
    public SaloesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar salões (público)</summary>
    /// <param name="cidade">Filtrar por cidade (ex: Porto Alegre)</param>
    /// <param name="busca">Busca por nome ou descrição</param>
    /// <response code="200">Lista de salões</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<SalaoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string? cidade, [FromQuery] string? busca)
    {
        var result = await _mediator.Send(new ListarSaloesQuery(cidade, busca));
        return Ok(result);
    }

    /// <summary>Obter salão por ID (público)</summary>
    /// <response code="200">Salão encontrado</response>
    /// <response code="404">Salão não encontrado</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SalaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var result = await _mediator.Send(new ObterSalaoQuery(id));
        return Ok(result);
    }

    /// <summary>Meus salões (somente proprietário autenticado)</summary>
    /// <response code="200">Lista de salões do proprietário</response>
    /// <response code="401">Não autenticado</response>
    [HttpGet("meus")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<SalaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MeusSaloes()
    {
        var result = await _mediator.Send(new ListarSaloesPorProprietarioQuery(UsuarioId));
        return Ok(result);
    }

    /// <summary>Criar novo salão</summary>
    /// <response code="201">Salão criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Não autenticado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SalaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Criar([FromBody] CriarSalaoRequest request)
    {
        var result = await _mediator.Send(new CriarSalaoCommand(
            UsuarioId, request.Nome, request.Descricao, request.Endereco,
            request.Cidade, request.Estado, request.Cep,
            request.Latitude, request.Longitude, request.Telefone,
            request.Email, request.HorarioFuncionamento));
        return Created($"/api/saloes/{result.Id}", result);
    }

    /// <summary>Atualizar salão (somente proprietário)</summary>
    /// <response code="200">Salão atualizado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não é o proprietário</response>
    /// <response code="404">Salão não encontrado</response>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(SalaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarSalaoRequest request)
    {
        var result = await _mediator.Send(new AtualizarSalaoCommand(
            id, UsuarioId, request.Nome, request.Descricao, request.Endereco,
            request.Cidade, request.Estado, request.Cep,
            request.Latitude, request.Longitude, request.Telefone,
            request.Email, request.HorarioFuncionamento));
        return Ok(result);
    }

    /// <summary>Desativar salão (somente proprietário)</summary>
    /// <response code="204">Salão desativado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="404">Salão não encontrado</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id)
    {
        await _mediator.Send(new DeletarSalaoCommand(id, UsuarioId));
        return NoContent();
    }
}
