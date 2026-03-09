using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Saloes;

// ── Criar Salão ─────────────────────────────────────────────────────────────
public record CriarSalaoCommand(int ProprietarioId, string Nome, string? Descricao,
    string Endereco, string Cidade, string Estado, string? Cep,
    decimal? Latitude, decimal? Longitude, string? Telefone,
    string? Email, string? HorarioFuncionamento) : IRequest<SalaoDto>;

public class CriarSalaoCommandValidator : AbstractValidator<CriarSalaoCommand>
{
    public CriarSalaoCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Endereco).NotEmpty();
        RuleFor(x => x.Cidade).NotEmpty();
        RuleFor(x => x.Estado).NotEmpty().Length(2);
    }
}

public class CriarSalaoCommandHandler : IRequestHandler<CriarSalaoCommand, SalaoDto>
{
    private readonly ISalaoRepository _repo;

    public CriarSalaoCommandHandler(ISalaoRepository repo) => _repo = repo;

    public async Task<SalaoDto> Handle(CriarSalaoCommand request, CancellationToken ct)
    {
        var salao = new Salao
        {
            ProprietarioId = request.ProprietarioId,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Endereco = request.Endereco,
            Cidade = request.Cidade,
            Estado = request.Estado,
            Cep = request.Cep,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Telefone = request.Telefone,
            Email = request.Email,
            HorarioFuncionamento = request.HorarioFuncionamento
        };

        var id = await _repo.CriarAsync(salao);
        salao.Id = id;

        return MapToDto(salao);
    }

    private static SalaoDto MapToDto(Salao s) =>
        new(s.Id, s.ProprietarioId, s.Nome, s.Descricao, s.LogoUrl,
            s.Endereco, s.Cidade, s.Estado, s.Telefone, s.Latitude, s.Longitude, null, s.Ativo);
}

// ── Atualizar Salão ─────────────────────────────────────────────────────────
public record AtualizarSalaoCommand(int Id, int ProprietarioId, string Nome, string? Descricao,
    string Endereco, string Cidade, string Estado, string? Cep,
    decimal? Latitude, decimal? Longitude, string? Telefone,
    string? Email, string? HorarioFuncionamento) : IRequest<SalaoDto>;

public class AtualizarSalaoCommandHandler : IRequestHandler<AtualizarSalaoCommand, SalaoDto>
{
    private readonly ISalaoRepository _repo;

    public AtualizarSalaoCommandHandler(ISalaoRepository repo) => _repo = repo;

    public async Task<SalaoDto> Handle(AtualizarSalaoCommand request, CancellationToken ct)
    {
        var salao = await _repo.ObterPorIdAsync(request.Id)
            ?? throw new NotFoundException("Salão", request.Id);

        if (salao.ProprietarioId != request.ProprietarioId)
            throw new UnauthorizedException("Você não tem permissão para editar este salão.");

        salao.Nome = request.Nome;
        salao.Descricao = request.Descricao;
        salao.Endereco = request.Endereco;
        salao.Cidade = request.Cidade;
        salao.Estado = request.Estado;
        salao.Cep = request.Cep;
        salao.Latitude = request.Latitude;
        salao.Longitude = request.Longitude;
        salao.Telefone = request.Telefone;
        salao.Email = request.Email;
        salao.HorarioFuncionamento = request.HorarioFuncionamento;
        salao.AtualizadoEm = DateTime.UtcNow;

        await _repo.AtualizarAsync(salao);

        return new SalaoDto(salao.Id, salao.ProprietarioId, salao.Nome, salao.Descricao, salao.LogoUrl,
            salao.Endereco, salao.Cidade, salao.Estado, salao.Telefone,
            salao.Latitude, salao.Longitude, null, salao.Ativo);
    }
}

// ── Deletar Salão ───────────────────────────────────────────────────────────
public record DeletarSalaoCommand(int Id, int ProprietarioId) : IRequest<Unit>;

public class DeletarSalaoCommandHandler : IRequestHandler<DeletarSalaoCommand, Unit>
{
    private readonly ISalaoRepository _repo;

    public DeletarSalaoCommandHandler(ISalaoRepository repo) => _repo = repo;

    public async Task<Unit> Handle(DeletarSalaoCommand request, CancellationToken ct)
    {
        var salao = await _repo.ObterPorIdAsync(request.Id)
            ?? throw new NotFoundException("Salão", request.Id);

        if (salao.ProprietarioId != request.ProprietarioId)
            throw new UnauthorizedException();

        await _repo.DeletarAsync(request.Id);
        return Unit.Value;
    }
}
