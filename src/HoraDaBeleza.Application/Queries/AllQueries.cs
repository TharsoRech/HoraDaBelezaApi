using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Queries.Usuarios
{
    public record ObterPerfilQuery(int UsuarioId) : IRequest<UsuarioDto>;

    public class ObterPerfilQueryHandler : IRequestHandler<ObterPerfilQuery, UsuarioDto>
    {
        private readonly IUsuarioRepository _repo;
        public ObterPerfilQueryHandler(IUsuarioRepository repo) => _repo = repo;

        public async Task<UsuarioDto> Handle(ObterPerfilQuery request, CancellationToken ct)
        {
            var u = await _repo.ObterPorIdAsync(request.UsuarioId)
                    ?? throw new NotFoundException("Usuário", request.UsuarioId);
            return new UsuarioDto(u.Id, u.Nome, u.Email, u.Telefone, u.FotoUrl, u.Tipo, u.Ativo);
        }
    }
}

namespace HoraDaBeleza.Application.Queries.Saloes
{
    public record ListarSaloesQuery(string? Cidade, string? Busca) : IRequest<IEnumerable<SalaoDto>>;

    public class ListarSaloesQueryHandler : IRequestHandler<ListarSaloesQuery, IEnumerable<SalaoDto>>
    {
        private readonly ISalaoRepository _repo;
        public ListarSaloesQueryHandler(ISalaoRepository repo) => _repo = repo;

        public async Task<IEnumerable<SalaoDto>> Handle(ListarSaloesQuery request, CancellationToken ct)
        {
            var saloes = await _repo.ListarAsync(request.Cidade, request.Busca);
            return saloes.Select(s => new SalaoDto(s.Id, s.ProprietarioId, s.Nome, s.Descricao, s.LogoUrl,
                s.Endereco, s.Cidade, s.Estado, s.Telefone, s.Latitude, s.Longitude, null, s.Ativo));
        }
    }

    public record ObterSalaoQuery(int Id) : IRequest<SalaoDto>;

    public class ObterSalaoQueryHandler : IRequestHandler<ObterSalaoQuery, SalaoDto>
    {
        private readonly ISalaoRepository _repo;
        public ObterSalaoQueryHandler(ISalaoRepository repo) => _repo = repo;

        public async Task<SalaoDto> Handle(ObterSalaoQuery request, CancellationToken ct)
        {
            var s = await _repo.ObterPorIdAsync(request.Id)
                    ?? throw new NotFoundException("Salão", request.Id);
            return new SalaoDto(s.Id, s.ProprietarioId, s.Nome, s.Descricao, s.LogoUrl,
                s.Endereco, s.Cidade, s.Estado, s.Telefone, s.Latitude, s.Longitude, null, s.Ativo);
        }
    }

    public record ListarSaloesPorProprietarioQuery(int ProprietarioId) : IRequest<IEnumerable<SalaoDto>>;

    public class ListarSaloesPorProprietarioQueryHandler : IRequestHandler<ListarSaloesPorProprietarioQuery, IEnumerable<SalaoDto>>
    {
        private readonly ISalaoRepository _repo;
        public ListarSaloesPorProprietarioQueryHandler(ISalaoRepository repo) => _repo = repo;

        public async Task<IEnumerable<SalaoDto>> Handle(ListarSaloesPorProprietarioQuery request, CancellationToken ct)
        {
            var saloes = await _repo.ListarPorProprietarioAsync(request.ProprietarioId);
            return saloes.Select(s => new SalaoDto(s.Id, s.ProprietarioId, s.Nome, s.Descricao, s.LogoUrl,
                s.Endereco, s.Cidade, s.Estado, s.Telefone, s.Latitude, s.Longitude, null, s.Ativo));
        }
    }
}

namespace HoraDaBeleza.Application.Queries.Servicos
{
    public record ListarServicosQuery(int SalaoId, int? CategoriaId = null) : IRequest<IEnumerable<ServicoDto>>;

    public class ListarServicosQueryHandler : IRequestHandler<ListarServicosQuery, IEnumerable<ServicoDto>>
    {
        private readonly IServicoRepository _repo;
        public ListarServicosQueryHandler(IServicoRepository repo) => _repo = repo;

        public async Task<IEnumerable<ServicoDto>> Handle(ListarServicosQuery request, CancellationToken ct)
        {
            var servicos = await _repo.ListarPorSalaoAsync(request.SalaoId, request.CategoriaId);
            return servicos.Select(s => new ServicoDto(s.Id, s.SalaoId, s.CategoriaId, "",
                s.Nome, s.Descricao, s.Preco, s.DuracaoMinutos, s.Ativo));
        }
    }
}

namespace HoraDaBeleza.Application.Queries.Profissionais
{
    public record ListarProfissionaisQuery(int SalaoId) : IRequest<IEnumerable<ProfissionalDto>>;

    public class ListarProfissionaisQueryHandler : IRequestHandler<ListarProfissionaisQuery, IEnumerable<ProfissionalDto>>
    {
        private readonly IProfissionalRepository _repo;
        public ListarProfissionaisQueryHandler(IProfissionalRepository repo) => _repo = repo;

        public async Task<IEnumerable<ProfissionalDto>> Handle(ListarProfissionaisQuery request, CancellationToken ct)
        {
            var profissionais = await _repo.ListarPorSalaoAsync(request.SalaoId);
            return profissionais.Select(p => new ProfissionalDto(p.Id, p.UsuarioId, p.SalaoId, "",
                null, p.Especialidade, p.Biografia, p.NotaMedia, p.TotalAvaliacoes, p.Ativo));
        }
    }
}

namespace HoraDaBeleza.Application.Queries.Agendamentos
{
    public record ListarAgendamentosClienteQuery(int ClienteId) : IRequest<IEnumerable<AgendamentoDto>>;

    public class ListarAgendamentosClienteQueryHandler : IRequestHandler<ListarAgendamentosClienteQuery, IEnumerable<AgendamentoDto>>
    {
        private readonly IAgendamentoRepository _repo;
        public ListarAgendamentosClienteQueryHandler(IAgendamentoRepository repo) => _repo = repo;

        public async Task<IEnumerable<AgendamentoDto>> Handle(ListarAgendamentosClienteQuery request, CancellationToken ct)
        {
            var agendamentos = await _repo.ListarPorClienteAsync(request.ClienteId);
            return agendamentos.Select(a => new AgendamentoDto(a.Id, a.ClienteId, "", a.ProfissionalId, "",
                a.ServicoId, "", a.SalaoId, "", a.DataHora, a.DuracaoMinutos, a.ValorTotal,
                a.Status, a.Observacoes, a.CriadoEm));
        }
    }

    public record ListarAgendamentosProfissionalQuery(int ProfissionalId, DateTime? Data) : IRequest<IEnumerable<AgendamentoDto>>;

    public class ListarAgendamentosProfissionalQueryHandler : IRequestHandler<ListarAgendamentosProfissionalQuery, IEnumerable<AgendamentoDto>>
    {
        private readonly IAgendamentoRepository _repo;
        public ListarAgendamentosProfissionalQueryHandler(IAgendamentoRepository repo) => _repo = repo;

        public async Task<IEnumerable<AgendamentoDto>> Handle(ListarAgendamentosProfissionalQuery request, CancellationToken ct)
        {
            var agendamentos = await _repo.ListarPorProfissionalAsync(request.ProfissionalId, request.Data);
            return agendamentos.Select(a => new AgendamentoDto(a.Id, a.ClienteId, "", a.ProfissionalId, "",
                a.ServicoId, "", a.SalaoId, "", a.DataHora, a.DuracaoMinutos, a.ValorTotal,
                a.Status, a.Observacoes, a.CriadoEm));
        }
    }

    public record ListarAgendamentosSalaoQuery(int SalaoId, DateTime? Data) : IRequest<IEnumerable<AgendamentoDto>>;

    public class ListarAgendamentosSalaoQueryHandler : IRequestHandler<ListarAgendamentosSalaoQuery, IEnumerable<AgendamentoDto>>
    {
        private readonly IAgendamentoRepository _repo;
        public ListarAgendamentosSalaoQueryHandler(IAgendamentoRepository repo) => _repo = repo;

        public async Task<IEnumerable<AgendamentoDto>> Handle(ListarAgendamentosSalaoQuery request, CancellationToken ct)
        {
            var agendamentos = await _repo.ListarPorSalaoAsync(request.SalaoId, request.Data);
            return agendamentos.Select(a => new AgendamentoDto(a.Id, a.ClienteId, "", a.ProfissionalId, "",
                a.ServicoId, "", a.SalaoId, "", a.DataHora, a.DuracaoMinutos, a.ValorTotal,
                a.Status, a.Observacoes, a.CriadoEm));
        }
    }
}

namespace HoraDaBeleza.Application.Queries.Avaliacoes
{
    public record ListarAvaliacoesSalaoQuery(int SalaoId) : IRequest<IEnumerable<AvaliacaoDto>>;

    public class ListarAvaliacoesSalaoQueryHandler : IRequestHandler<ListarAvaliacoesSalaoQuery, IEnumerable<AvaliacaoDto>>
    {
        private readonly IAvaliacaoRepository _repo;
        public ListarAvaliacoesSalaoQueryHandler(IAvaliacaoRepository repo) => _repo = repo;

        public async Task<IEnumerable<AvaliacaoDto>> Handle(ListarAvaliacoesSalaoQuery request, CancellationToken ct)
        {
            var avaliacoes = await _repo.ListarPorSalaoAsync(request.SalaoId);
            return avaliacoes.Select(a => new AvaliacaoDto(a.Id, a.AgendamentoId, "",
                a.Nota, a.Comentario, a.CriadoEm));
        }
    }
}

namespace HoraDaBeleza.Application.Queries.Planos
{
    public record ListarPlanosQuery : IRequest<IEnumerable<PlanoDto>>;

    public class ListarPlanosQueryHandler : IRequestHandler<ListarPlanosQuery, IEnumerable<PlanoDto>>
    {
        private readonly IPlanoRepository _repo;
        public ListarPlanosQueryHandler(IPlanoRepository repo) => _repo = repo;

        public async Task<IEnumerable<PlanoDto>> Handle(ListarPlanosQuery request, CancellationToken ct)
        {
            var planos = await _repo.ListarAtivosAsync();
            return planos.Select(p => new PlanoDto(p.Id, p.Nome, p.Descricao, p.Preco,
                p.PeriodoDias, p.LimiteAgendamentos));
        }
    }
}

namespace HoraDaBeleza.Application.Queries.Usuarios
{
    public record ListarNotificacoesQuery(int UsuarioId, bool ApenasNaoLidas = false) : IRequest<IEnumerable<NotificacaoDto>>;

    public class ListarNotificacoesQueryHandler : IRequestHandler<ListarNotificacoesQuery, IEnumerable<NotificacaoDto>>
    {
        private readonly INotificacaoRepository _repo;
        public ListarNotificacoesQueryHandler(INotificacaoRepository repo) => _repo = repo;

        public async Task<IEnumerable<NotificacaoDto>> Handle(ListarNotificacoesQuery request, CancellationToken ct)
        {
            var notificacoes = await _repo.ListarPorUsuarioAsync(request.UsuarioId, request.ApenasNaoLidas);
            return notificacoes.Select(n => new NotificacaoDto(n.Id, n.Titulo, n.Mensagem,
                n.Tipo, n.Lida, n.ReferenciaId, n.CriadoEm));
        }
    }
}
