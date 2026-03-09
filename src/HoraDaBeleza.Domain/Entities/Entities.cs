using HoraDaBeleza.Domain.Enums;

namespace HoraDaBeleza.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? FotoUrl { get; set; }
    public TipoUsuario Tipo { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}

public class Salao
{
    public int Id { get; set; }
    public int ProprietarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? LogoUrl { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Cep { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? HorarioFuncionamento { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}

public class Profissional
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int SalaoId { get; set; }
    public string? Especialidade { get; set; }
    public string? Biografia { get; set; }
    public decimal? NotaMedia { get; set; }
    public int TotalAvaliacoes { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? IconeUrl { get; set; }
    public bool Ativo { get; set; } = true;
}

public class Servico
{
    public int Id { get; set; }
    public int SalaoId { get; set; }
    public int CategoriaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int DuracaoMinutos { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

public class Agendamento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ProfissionalId { get; set; }
    public int ServicoId { get; set; }
    public int SalaoId { get; set; }
    public DateTime DataHora { get; set; }
    public int DuracaoMinutos { get; set; }
    public decimal ValorTotal { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Pendente;
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}

public class Avaliacao
{
    public int Id { get; set; }
    public int AgendamentoId { get; set; }
    public int ClienteId { get; set; }
    public int ProfissionalId { get; set; }
    public int SalaoId { get; set; }
    public int Nota { get; set; } // 1-5
    public string? Comentario { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

public class Plano
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int PeriodoDias { get; set; }
    public int LimiteAgendamentos { get; set; }
    public bool Ativo { get; set; } = true;
}

public class Assinatura
{
    public int Id { get; set; }
    public int SalaoId { get; set; }
    public int PlanoId { get; set; }
    public StatusAssinatura Status { get; set; } = StatusAssinatura.Ativa;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

public class Notificacao
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public TipoNotificacao Tipo { get; set; }
    public bool Lida { get; set; } = false;
    public int? ReferenciaId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
