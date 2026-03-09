using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Application.DTOs;

// Auth
public record LoginRequest(string Email, string Senha);
public record LoginResponse(string Token, string Nome, string Email, TipoUsuario Tipo);
public record RegistroRequest(string Nome, string Email, string Senha, string? Telefone, TipoUsuario Tipo);

// Usuario
public record UsuarioDto(int Id, string Nome, string Email, string? Telefone, string? FotoUrl, TipoUsuario Tipo, bool Ativo);

// Salao
public record SalaoDto(int Id, int ProprietarioId, string Nome, string? Descricao, string? LogoUrl,
    string Endereco, string Cidade, string Estado, string? Telefone, decimal? Latitude, decimal? Longitude,
    decimal? NotaMedia, bool Ativo);

public record CriarSalaoRequest(string Nome, string? Descricao, string Endereco, string Cidade,
    string Estado, string? Cep, decimal? Latitude, decimal? Longitude, string? Telefone, string? Email, string? HorarioFuncionamento);

public record AtualizarSalaoRequest(string Nome, string? Descricao, string Endereco, string Cidade,
    string Estado, string? Cep, decimal? Latitude, decimal? Longitude, string? Telefone, string? Email, string? HorarioFuncionamento);

// Profissional
public record ProfissionalDto(int Id, int UsuarioId, int SalaoId, string NomeUsuario, string? FotoUrl,
    string? Especialidade, string? Biografia, decimal? NotaMedia, int TotalAvaliacoes, bool Ativo);

public record CriarProfissionalRequest(int UsuarioId, int SalaoId, string? Especialidade, string? Biografia);

// Servico
public record ServicoDto(int Id, int SalaoId, int CategoriaId, string NomeCategoria, string Nome,
    string? Descricao, decimal Preco, int DuracaoMinutos, bool Ativo);

public record CriarServicoRequest(int CategoriaId, string Nome, string? Descricao, decimal Preco, int DuracaoMinutos);
public record AtualizarServicoRequest(string Nome, string? Descricao, decimal Preco, int DuracaoMinutos, bool Ativo);

// Categoria
public record CategoriaDto(int Id, string Nome, string? IconeUrl);

// Agendamento
public record AgendamentoDto(int Id, int ClienteId, string NomeCliente, int ProfissionalId, string NomeProfissional,
    int ServicoId, string NomeServico, int SalaoId, string NomeSalao, DateTime DataHora,
    int DuracaoMinutos, decimal ValorTotal, StatusAgendamento Status, string? Observacoes, DateTime CriadoEm);

public record CriarAgendamentoRequest(int ProfissionalId, int ServicoId, int SalaoId,
    DateTime DataHora, string? Observacoes);

public record AtualizarStatusAgendamentoRequest(StatusAgendamento Status);

// Avaliacao
public record AvaliacaoDto(int Id, int AgendamentoId, string NomeCliente, int Nota,
    string? Comentario, DateTime CriadoEm);

public record CriarAvaliacaoRequest(int AgendamentoId, int Nota, string? Comentario);

// Plano
public record PlanoDto(int Id, string Nome, string? Descricao, decimal Preco,
    int PeriodoDias, int LimiteAgendamentos);

// Assinatura
public record AssinaturaDto(int Id, int SalaoId, int PlanoId, string NomePlano,
    Domain.Enums.StatusAssinatura Status, DateTime DataInicio, DateTime DataFim);

public record CriarAssinaturaRequest(int SalaoId, int PlanoId);

// Notificacao
public record NotificacaoDto(int Id, string Titulo, string Mensagem,
    Domain.Enums.TipoNotificacao Tipo, bool Lida, int? ReferenciaId, DateTime CriadoEm);

// Horarios disponíveis
public record HorarioDisponivelDto(DateTime DataHora, bool Disponivel);

// Paginação
public record PagedResult<T>(IEnumerable<T> Itens, int Total, int Pagina, int TamanhoPagina);
