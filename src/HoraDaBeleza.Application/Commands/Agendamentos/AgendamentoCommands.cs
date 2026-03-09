using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Agendamentos;

// ── Criar Agendamento ───────────────────────────────────────────────────────
public record CriarAgendamentoCommand(int ClienteId, int ProfissionalId, int ServicoId,
    int SalaoId, DateTime DataHora, string? Observacoes) : IRequest<AgendamentoDto>;

public class CriarAgendamentoCommandValidator : AbstractValidator<CriarAgendamentoCommand>
{
    public CriarAgendamentoCommandValidator()
    {
        RuleFor(x => x.ProfissionalId).GreaterThan(0);
        RuleFor(x => x.ServicoId).GreaterThan(0);
        RuleFor(x => x.SalaoId).GreaterThan(0);
        RuleFor(x => x.DataHora).GreaterThan(DateTime.UtcNow).WithMessage("A data do agendamento deve ser futura.");
    }
}

public class CriarAgendamentoCommandHandler : IRequestHandler<CriarAgendamentoCommand, AgendamentoDto>
{
    private readonly IAgendamentoRepository _agendamentoRepo;
    private readonly IServicoRepository _servicoRepo;
    private readonly IProfissionalRepository _profissionalRepo;
    private readonly ISalaoRepository _salaoRepo;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly INotificacaoRepository _notificacaoRepo;

    public CriarAgendamentoCommandHandler(
        IAgendamentoRepository agendamentoRepo,
        IServicoRepository servicoRepo,
        IProfissionalRepository profissionalRepo,
        ISalaoRepository salaoRepo,
        IUsuarioRepository usuarioRepo,
        INotificacaoRepository notificacaoRepo)
    {
        _agendamentoRepo = agendamentoRepo;
        _servicoRepo = servicoRepo;
        _profissionalRepo = profissionalRepo;
        _salaoRepo = salaoRepo;
        _usuarioRepo = usuarioRepo;
        _notificacaoRepo = notificacaoRepo;
    }

    public async Task<AgendamentoDto> Handle(CriarAgendamentoCommand request, CancellationToken ct)
    {
        var servico = await _servicoRepo.ObterPorIdAsync(request.ServicoId)
            ?? throw new NotFoundException("Serviço", request.ServicoId);

        if (!servico.Ativo)
            throw new BusinessException("Serviço indisponível.");

        var profissional = await _profissionalRepo.ObterPorIdAsync(request.ProfissionalId)
            ?? throw new NotFoundException("Profissional", request.ProfissionalId);

        if (profissional.SalaoId != request.SalaoId)
            throw new BusinessException("Profissional não pertence a este salão.");

        var conflito = await _agendamentoRepo.VerificarConflitoAsync(
            request.ProfissionalId, request.DataHora, servico.DuracaoMinutos);

        if (conflito)
            throw new BusinessException("Horário indisponível. Por favor, escolha outro horário.");

        var agendamento = new Agendamento
        {
            ClienteId = request.ClienteId,
            ProfissionalId = request.ProfissionalId,
            ServicoId = request.ServicoId,
            SalaoId = request.SalaoId,
            DataHora = request.DataHora,
            DuracaoMinutos = servico.DuracaoMinutos,
            ValorTotal = servico.Preco,
            Status = StatusAgendamento.Pendente,
            Observacoes = request.Observacoes
        };

        var id = await _agendamentoRepo.CriarAsync(agendamento);

        // Notificar o profissional
        var salao = await _salaoRepo.ObterPorIdAsync(request.SalaoId);
        var cliente = await _usuarioRepo.ObterPorIdAsync(request.ClienteId);
        var profissionalUsuario = await _usuarioRepo.ObterPorIdAsync(profissional.UsuarioId);

        if (profissionalUsuario != null && cliente != null && salao != null)
        {
            await _notificacaoRepo.CriarAsync(new Notificacao
            {
                UsuarioId = profissionalUsuario.Id,
                Titulo = "Novo agendamento",
                Mensagem = $"{cliente.Nome} agendou {servico.Nome} para {request.DataHora:dd/MM/yyyy HH:mm}.",
                Tipo = TipoNotificacao.AgendamentoConfirmado,
                ReferenciaId = id
            });
        }

        return new AgendamentoDto(id, request.ClienteId, cliente?.Nome ?? "",
            request.ProfissionalId, profissionalUsuario?.Nome ?? "",
            request.ServicoId, servico.Nome, request.SalaoId, salao?.Nome ?? "",
            request.DataHora, servico.DuracaoMinutos, servico.Preco,
            StatusAgendamento.Pendente, request.Observacoes, DateTime.UtcNow);
    }
}

// ── Cancelar Agendamento ────────────────────────────────────────────────────
public record CancelarAgendamentoCommand(int Id, int UsuarioId) : IRequest<Unit>;

public class CancelarAgendamentoCommandHandler : IRequestHandler<CancelarAgendamentoCommand, Unit>
{
    private readonly IAgendamentoRepository _repo;
    private readonly INotificacaoRepository _notificacaoRepo;
    private readonly IProfissionalRepository _profissionalRepo;
    private readonly IUsuarioRepository _usuarioRepo;

    public CancelarAgendamentoCommandHandler(IAgendamentoRepository repo,
        INotificacaoRepository notificacaoRepo, IProfissionalRepository profissionalRepo,
        IUsuarioRepository usuarioRepo)
    {
        _repo = repo;
        _notificacaoRepo = notificacaoRepo;
        _profissionalRepo = profissionalRepo;
        _usuarioRepo = usuarioRepo;
    }

    public async Task<Unit> Handle(CancelarAgendamentoCommand request, CancellationToken ct)
    {
        var agendamento = await _repo.ObterPorIdAsync(request.Id)
            ?? throw new NotFoundException("Agendamento", request.Id);

        var profissional = await _profissionalRepo.ObterPorIdAsync(agendamento.ProfissionalId);
        bool isCliente = agendamento.ClienteId == request.UsuarioId;
        bool isProfissional = profissional?.UsuarioId == request.UsuarioId;

        if (!isCliente && !isProfissional)
            throw new UnauthorizedException();

        if (agendamento.Status == StatusAgendamento.Concluido)
            throw new BusinessException("Não é possível cancelar um agendamento já concluído.");

        if (agendamento.Status == StatusAgendamento.Cancelado)
            throw new BusinessException("Agendamento já cancelado.");

        await _repo.AtualizarStatusAsync(request.Id, StatusAgendamento.Cancelado);

        // Notificar a outra parte
        int notificarUsuarioId = isCliente ? (profissional?.UsuarioId ?? 0) : agendamento.ClienteId;
        if (notificarUsuarioId > 0)
        {
            var quemCancelou = await _usuarioRepo.ObterPorIdAsync(request.UsuarioId);
            await _notificacaoRepo.CriarAsync(new Notificacao
            {
                UsuarioId = notificarUsuarioId,
                Titulo = "Agendamento cancelado",
                Mensagem = $"{quemCancelou?.Nome} cancelou o agendamento de {agendamento.DataHora:dd/MM/yyyy HH:mm}.",
                Tipo = TipoNotificacao.AgendamentoCancelado,
                ReferenciaId = request.Id
            });
        }

        return Unit.Value;
    }
}

// ── Confirmar/Concluir Agendamento ──────────────────────────────────────────
public record AtualizarStatusAgendamentoCommand(int Id, int ProfissionalUsuarioId,
    StatusAgendamento NovoStatus) : IRequest<Unit>;

public class AtualizarStatusAgendamentoCommandHandler : IRequestHandler<AtualizarStatusAgendamentoCommand, Unit>
{
    private readonly IAgendamentoRepository _repo;
    private readonly IProfissionalRepository _profissionalRepo;

    public AtualizarStatusAgendamentoCommandHandler(IAgendamentoRepository repo,
        IProfissionalRepository profissionalRepo)
    {
        _repo = repo;
        _profissionalRepo = profissionalRepo;
    }

    public async Task<Unit> Handle(AtualizarStatusAgendamentoCommand request, CancellationToken ct)
    {
        var agendamento = await _repo.ObterPorIdAsync(request.Id)
            ?? throw new NotFoundException("Agendamento", request.Id);

        var profissional = await _profissionalRepo.ObterPorIdAsync(agendamento.ProfissionalId);
        if (profissional?.UsuarioId != request.ProfissionalUsuarioId)
            throw new UnauthorizedException();

        await _repo.AtualizarStatusAsync(request.Id, request.NovoStatus);
        return Unit.Value;
    }
}
