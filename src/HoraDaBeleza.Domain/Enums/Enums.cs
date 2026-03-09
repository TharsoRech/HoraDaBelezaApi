namespace HoraDaBeleza.Domain.Enums;

public enum StatusAgendamento
{
    Pendente = 1,
    Confirmado = 2,
    Cancelado = 3,
    Concluido = 4,
    NaoCompareceu = 5
}

public enum TipoUsuario
{
    Cliente = 1,
    Profissional = 2,
    Proprietario = 3,
    Admin = 4
}

public enum StatusAssinatura
{
    Ativa = 1,
    Cancelada = 2,
    Expirada = 3,
    Suspensa = 4
}

public enum TipoNotificacao
{
    AgendamentoConfirmado = 1,
    AgendamentoCancelado = 2,
    AgendamentoLembrete = 3,
    NovaAvaliacao = 4,
    Promocao = 5,
    Sistema = 6
}
