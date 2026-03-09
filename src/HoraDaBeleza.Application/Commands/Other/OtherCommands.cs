using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Servicos
{
    public record CriarServicoCommand(int SalaoId, int ProprietarioId, int CategoriaId, string Nome,
        string? Descricao, decimal Preco, int DuracaoMinutos) : IRequest<ServicoDto>;

    public class CriarServicoCommandValidator : AbstractValidator<CriarServicoCommand>
    {
        public CriarServicoCommandValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Preco).GreaterThan(0);
            RuleFor(x => x.DuracaoMinutos).GreaterThan(0).LessThanOrEqualTo(480);
        }
    }

    public class CriarServicoCommandHandler : IRequestHandler<CriarServicoCommand, ServicoDto>
    {
        private readonly IServicoRepository _repo;
        private readonly ISalaoRepository _salaoRepo;
        private readonly ICategoriaRepository _categoriaRepo;

        public CriarServicoCommandHandler(IServicoRepository repo, ISalaoRepository salaoRepo, ICategoriaRepository categoriaRepo)
        {
            _repo = repo;
            _salaoRepo = salaoRepo;
            _categoriaRepo = categoriaRepo;
        }

        public async Task<ServicoDto> Handle(CriarServicoCommand request, CancellationToken ct)
        {
            var salao = await _salaoRepo.ObterPorIdAsync(request.SalaoId)
                        ?? throw new NotFoundException("Salão", request.SalaoId);

            if (salao.ProprietarioId != request.ProprietarioId)
                throw new UnauthorizedException();

            var categoria = await _categoriaRepo.ObterPorIdAsync(request.CategoriaId)
                            ?? throw new NotFoundException("Categoria", request.CategoriaId);

            var servico = new Servico
            {
                SalaoId = request.SalaoId,
                CategoriaId = request.CategoriaId,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                DuracaoMinutos = request.DuracaoMinutos
            };

            var id = await _repo.CriarAsync(servico);

            return new ServicoDto(id, request.SalaoId, request.CategoriaId, categoria.Nome,
                request.Nome, request.Descricao, request.Preco, request.DuracaoMinutos, true);
        }
    }

    public record AtualizarServicoCommand(int Id, int SalaoId, int ProprietarioId, string Nome,
        string? Descricao, decimal Preco, int DuracaoMinutos, bool Ativo) : IRequest<Unit>;

    public class AtualizarServicoCommandHandler : IRequestHandler<AtualizarServicoCommand, Unit>
    {
        private readonly IServicoRepository _repo;
        private readonly ISalaoRepository _salaoRepo;

        public AtualizarServicoCommandHandler(IServicoRepository repo, ISalaoRepository salaoRepo)
        {
            _repo = repo;
            _salaoRepo = salaoRepo;
        }

        public async Task<Unit> Handle(AtualizarServicoCommand request, CancellationToken ct)
        {
            var salao = await _salaoRepo.ObterPorIdAsync(request.SalaoId)
                        ?? throw new NotFoundException("Salão", request.SalaoId);

            if (salao.ProprietarioId != request.ProprietarioId)
                throw new UnauthorizedException();

            var servico = await _repo.ObterPorIdAsync(request.Id)
                          ?? throw new NotFoundException("Serviço", request.Id);

            servico.Nome = request.Nome;
            servico.Descricao = request.Descricao;
            servico.Preco = request.Preco;
            servico.DuracaoMinutos = request.DuracaoMinutos;
            servico.Ativo = request.Ativo;

            await _repo.AtualizarAsync(servico);
            return Unit.Value;
        }
    }

    public record DeletarServicoCommand(int Id, int SalaoId, int ProprietarioId) : IRequest<Unit>;

    public class DeletarServicoCommandHandler : IRequestHandler<DeletarServicoCommand, Unit>
    {
        private readonly IServicoRepository _repo;
        private readonly ISalaoRepository _salaoRepo;

        public DeletarServicoCommandHandler(IServicoRepository repo, ISalaoRepository salaoRepo)
        {
            _repo = repo;
            _salaoRepo = salaoRepo;
        }

        public async Task<Unit> Handle(DeletarServicoCommand request, CancellationToken ct)
        {
            var salao = await _salaoRepo.ObterPorIdAsync(request.SalaoId)
                        ?? throw new NotFoundException("Salão", request.SalaoId);

            if (salao.ProprietarioId != request.ProprietarioId)
                throw new UnauthorizedException();

            await _repo.DeletarAsync(request.Id);
            return Unit.Value;
        }
    }
}

namespace HoraDaBeleza.Application.Commands.Profissionais
{
    public record CriarProfissionalCommand(int UsuarioId, int SalaoId, int ProprietarioId,
        string? Especialidade, string? Biografia) : IRequest<ProfissionalDto>;

    public class CriarProfissionalCommandHandler : IRequestHandler<CriarProfissionalCommand, ProfissionalDto>
    {
        private readonly IProfissionalRepository _repo;
        private readonly ISalaoRepository _salaoRepo;
        private readonly IUsuarioRepository _usuarioRepo;

        public CriarProfissionalCommandHandler(IProfissionalRepository repo, ISalaoRepository salaoRepo, IUsuarioRepository usuarioRepo)
        {
            _repo = repo;
            _salaoRepo = salaoRepo;
            _usuarioRepo = usuarioRepo;
        }

        public async Task<ProfissionalDto> Handle(CriarProfissionalCommand request, CancellationToken ct)
        {
            var salao = await _salaoRepo.ObterPorIdAsync(request.SalaoId)
                        ?? throw new NotFoundException("Salão", request.SalaoId);

            if (salao.ProprietarioId != request.ProprietarioId)
                throw new UnauthorizedException();

            var usuario = await _usuarioRepo.ObterPorIdAsync(request.UsuarioId)
                          ?? throw new NotFoundException("Usuário", request.UsuarioId);

            var profissional = new Profissional
            {
                UsuarioId = request.UsuarioId,
                SalaoId = request.SalaoId,
                Especialidade = request.Especialidade,
                Biografia = request.Biografia
            };

            var id = await _repo.CriarAsync(profissional);

            return new ProfissionalDto(id, request.UsuarioId, request.SalaoId, usuario.Nome,
                usuario.FotoUrl, request.Especialidade, request.Biografia, null, 0, true);
        }
    }
}

namespace HoraDaBeleza.Application.Commands.Avaliacoes
{
    public record CriarAvaliacaoCommand(int AgendamentoId, int ClienteId, int Nota, string? Comentario) : IRequest<AvaliacaoDto>;

    public class CriarAvaliacaoCommandValidator : AbstractValidator<CriarAvaliacaoCommand>
    {
        public CriarAvaliacaoCommandValidator()
        {
            RuleFor(x => x.Nota).InclusiveBetween(1, 5).WithMessage("A nota deve ser entre 1 e 5.");
        }
    }

    public class CriarAvaliacaoCommandHandler : IRequestHandler<CriarAvaliacaoCommand, AvaliacaoDto>
    {
        private readonly IAvaliacaoRepository _avaliacaoRepo;
        private readonly IAgendamentoRepository _agendamentoRepo;
        private readonly IUsuarioRepository _usuarioRepo;

        public CriarAvaliacaoCommandHandler(IAvaliacaoRepository avaliacaoRepo,
            IAgendamentoRepository agendamentoRepo, IUsuarioRepository usuarioRepo)
        {
            _avaliacaoRepo = avaliacaoRepo;
            _agendamentoRepo = agendamentoRepo;
            _usuarioRepo = usuarioRepo;
        }

        public async Task<AvaliacaoDto> Handle(CriarAvaliacaoCommand request, CancellationToken ct)
        {
            var agendamento = await _agendamentoRepo.ObterPorIdAsync(request.AgendamentoId)
                              ?? throw new NotFoundException("Agendamento", request.AgendamentoId);

            if (agendamento.ClienteId != request.ClienteId)
                throw new UnauthorizedException("Você não pode avaliar este agendamento.");

            if (agendamento.Status != Domain.Enums.StatusAgendamento.Concluido)
                throw new BusinessException("Só é possível avaliar agendamentos concluídos.");

            if (await _avaliacaoRepo.ExisteAvaliacaoParaAgendamentoAsync(request.AgendamentoId))
                throw new BusinessException("Este agendamento já foi avaliado.");

            var cliente = await _usuarioRepo.ObterPorIdAsync(request.ClienteId);

            var avaliacao = new Avaliacao
            {
                AgendamentoId = request.AgendamentoId,
                ClienteId = request.ClienteId,
                ProfissionalId = agendamento.ProfissionalId,
                SalaoId = agendamento.SalaoId,
                Nota = request.Nota,
                Comentario = request.Comentario
            };

            var id = await _avaliacaoRepo.CriarAsync(avaliacao);

            return new AvaliacaoDto(id, request.AgendamentoId, cliente?.Nome ?? "",
                request.Nota, request.Comentario, DateTime.UtcNow);
        }
    }
}

namespace HoraDaBeleza.Application.Commands.Assinaturas
{
    public record CriarAssinaturaCommand(int SalaoId, int ProprietarioId, int PlanoId) : IRequest<AssinaturaDto>;

    public class CriarAssinaturaCommandHandler : IRequestHandler<CriarAssinaturaCommand, AssinaturaDto>
    {
        private readonly IAssinaturaRepository _repo;
        private readonly ISalaoRepository _salaoRepo;
        private readonly IPlanoRepository _planoRepo;

        public CriarAssinaturaCommandHandler(IAssinaturaRepository repo, ISalaoRepository salaoRepo, IPlanoRepository planoRepo)
        {
            _repo = repo;
            _salaoRepo = salaoRepo;
            _planoRepo = planoRepo;
        }

        public async Task<AssinaturaDto> Handle(CriarAssinaturaCommand request, CancellationToken ct)
        {
            var salao = await _salaoRepo.ObterPorIdAsync(request.SalaoId)
                        ?? throw new NotFoundException("Salão", request.SalaoId);

            if (salao.ProprietarioId != request.ProprietarioId)
                throw new UnauthorizedException();

            var plano = await _planoRepo.ObterPorIdAsync(request.PlanoId)
                        ?? throw new NotFoundException("Plano", request.PlanoId);

            // Cancelar assinatura ativa se existir
            var assinaturaAtiva = await _repo.ObterAtivaDoSalaoAsync(request.SalaoId);
            if (assinaturaAtiva != null)
            {
                assinaturaAtiva.Status = Domain.Enums.StatusAssinatura.Cancelada;
                await _repo.AtualizarAsync(assinaturaAtiva);
            }

            var agora = DateTime.UtcNow;
            var assinatura = new Assinatura
            {
                SalaoId = request.SalaoId,
                PlanoId = request.PlanoId,
                Status = Domain.Enums.StatusAssinatura.Ativa,
                DataInicio = agora,
                DataFim = agora.AddDays(plano.PeriodoDias)
            };

            var id = await _repo.CriarAsync(assinatura);

            return new AssinaturaDto(id, request.SalaoId, request.PlanoId, plano.Nome,
                Domain.Enums.StatusAssinatura.Ativa, assinatura.DataInicio, assinatura.DataFim);
        }
    }
}
