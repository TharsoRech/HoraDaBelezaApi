using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<int> CriarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task<bool> ExisteEmailAsync(string email, int? ignorarId = null);
}

public interface ISalaoRepository
{
    Task<Salao?> ObterPorIdAsync(int id);
    Task<IEnumerable<Salao>> ListarAsync(string? cidade = null, string? busca = null);
    Task<IEnumerable<Salao>> ListarPorProprietarioAsync(int proprietarioId);
    Task<int> CriarAsync(Salao salao);
    Task AtualizarAsync(Salao salao);
    Task DeletarAsync(int id);
}

public interface IProfissionalRepository
{
    Task<Profissional?> ObterPorIdAsync(int id);
    Task<IEnumerable<Profissional>> ListarPorSalaoAsync(int salaoId);
    Task<int> CriarAsync(Profissional profissional);
    Task AtualizarAsync(Profissional profissional);
    Task DeletarAsync(int id);
}

public interface IServicoRepository
{
    Task<Servico?> ObterPorIdAsync(int id);
    Task<IEnumerable<Servico>> ListarPorSalaoAsync(int salaoId, int? categoriaId = null);
    Task<int> CriarAsync(Servico servico);
    Task AtualizarAsync(Servico servico);
    Task DeletarAsync(int id);
}

public interface IAgendamentoRepository
{
    Task<Agendamento?> ObterPorIdAsync(int id);
    Task<IEnumerable<Agendamento>> ListarPorClienteAsync(int clienteId);
    Task<IEnumerable<Agendamento>> ListarPorProfissionalAsync(int profissionalId, DateTime? data = null);
    Task<IEnumerable<Agendamento>> ListarPorSalaoAsync(int salaoId, DateTime? data = null);
    Task<bool> VerificarConflitoAsync(int profissionalId, DateTime dataHora, int duracaoMinutos, int? ignorarId = null);
    Task<int> CriarAsync(Agendamento agendamento);
    Task AtualizarStatusAsync(int id, Domain.Enums.StatusAgendamento status);
}

public interface IAvaliacaoRepository
{
    Task<IEnumerable<Avaliacao>> ListarPorSalaoAsync(int salaoId);
    Task<IEnumerable<Avaliacao>> ListarPorProfissionalAsync(int profissionalId);
    Task<bool> ExisteAvaliacaoParaAgendamentoAsync(int agendamentoId);
    Task<int> CriarAsync(Avaliacao avaliacao);
}

public interface IPlanoRepository
{
    Task<IEnumerable<Plano>> ListarAtivosAsync();
    Task<Plano?> ObterPorIdAsync(int id);
}

public interface IAssinaturaRepository
{
    Task<Assinatura?> ObterAtivaDoSalaoAsync(int salaoId);
    Task<int> CriarAsync(Assinatura assinatura);
    Task AtualizarAsync(Assinatura assinatura);
}

public interface INotificacaoRepository
{
    Task<IEnumerable<Notificacao>> ListarPorUsuarioAsync(int usuarioId, bool apenasNaoLidas = false);
    Task<int> CriarAsync(Notificacao notificacao);
    Task MarcarComoLidaAsync(int id, int usuarioId);
    Task MarcarTodasComoLidasAsync(int usuarioId);
}

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> ListarAsync();
    Task<Categoria?> ObterPorIdAsync(int id);
}

public interface ITokenService
{
    string GerarToken(Domain.Entities.Usuario usuario);
}
