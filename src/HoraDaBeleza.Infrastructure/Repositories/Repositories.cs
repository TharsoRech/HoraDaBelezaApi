using Dapper;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Infrastructure.Data;

namespace HoraDaBeleza.Infrastructure.Repositories;

// ── Usuario ────────────────────────────────────────────────────────────────
public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _db;
    public UsuarioRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM Usuarios WHERE Id = @Id AND Ativo = 1", new { Id = id });
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM Usuarios WHERE Email = @Email", new { Email = email.ToLower() });
    }

    public async Task<int> CriarAsync(Usuario usuario)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Usuarios (Nome, Email, SenhaHash, Telefone, FotoUrl, Tipo, Ativo, CriadoEm)
                    VALUES (@Nome, @Email, @SenhaHash, @Telefone, @FotoUrl, @Tipo, @Ativo, @CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, usuario);
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        using var conn = _db.CreateConnection();
        var sql = @"UPDATE Usuarios SET Nome=@Nome, Telefone=@Telefone, FotoUrl=@FotoUrl,
                    AtualizadoEm=@AtualizadoEm WHERE Id=@Id";
        await conn.ExecuteAsync(sql, usuario);
    }

    public async Task<bool> ExisteEmailAsync(string email, int? ignorarId = null)
    {
        using var conn = _db.CreateConnection();
        var sql = ignorarId.HasValue
            ? "SELECT COUNT(1) FROM Usuarios WHERE Email=@Email AND Id<>@IgnorarId"
            : "SELECT COUNT(1) FROM Usuarios WHERE Email=@Email";
        return await conn.QuerySingleAsync<int>(sql, new { Email = email.ToLower(), IgnorarId = ignorarId }) > 0;
    }
}

// ── Salao ──────────────────────────────────────────────────────────────────
public class SalaoRepository : ISalaoRepository
{
    private readonly IDbConnectionFactory _db;
    public SalaoRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Salao?> ObterPorIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Salao>(
            "SELECT * FROM Saloes WHERE Id=@Id AND Ativo=1", new { Id = id });
    }

    public async Task<IEnumerable<Salao>> ListarAsync(string? cidade = null, string? busca = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Saloes WHERE Ativo=1";
        if (!string.IsNullOrWhiteSpace(cidade)) sql += " AND Cidade=@Cidade";
        if (!string.IsNullOrWhiteSpace(busca)) sql += " AND (Nome LIKE @Busca OR Descricao LIKE @Busca)";
        sql += " ORDER BY Nome";
        return await conn.QueryAsync<Salao>(sql, new { Cidade = cidade, Busca = $"%{busca}%" });
    }

    public async Task<IEnumerable<Salao>> ListarPorProprietarioAsync(int proprietarioId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Salao>(
            "SELECT * FROM Saloes WHERE ProprietarioId=@ProprietarioId AND Ativo=1 ORDER BY Nome",
            new { ProprietarioId = proprietarioId });
    }

    public async Task<int> CriarAsync(Salao salao)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Saloes (ProprietarioId,Nome,Descricao,LogoUrl,Endereco,Cidade,Estado,Cep,
                    Latitude,Longitude,Telefone,Email,HorarioFuncionamento,Ativo,CriadoEm)
                    VALUES (@ProprietarioId,@Nome,@Descricao,@LogoUrl,@Endereco,@Cidade,@Estado,@Cep,
                    @Latitude,@Longitude,@Telefone,@Email,@HorarioFuncionamento,@Ativo,@CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, salao);
    }

    public async Task AtualizarAsync(Salao salao)
    {
        using var conn = _db.CreateConnection();
        var sql = @"UPDATE Saloes SET Nome=@Nome,Descricao=@Descricao,Endereco=@Endereco,
                    Cidade=@Cidade,Estado=@Estado,Cep=@Cep,Latitude=@Latitude,Longitude=@Longitude,
                    Telefone=@Telefone,Email=@Email,HorarioFuncionamento=@HorarioFuncionamento,
                    AtualizadoEm=@AtualizadoEm WHERE Id=@Id";
        await conn.ExecuteAsync(sql, salao);
    }

    public async Task DeletarAsync(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Saloes SET Ativo=0 WHERE Id=@Id", new { Id = id });
    }
}

// ── Profissional ───────────────────────────────────────────────────────────
public class ProfissionalRepository : IProfissionalRepository
{
    private readonly IDbConnectionFactory _db;
    public ProfissionalRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Profissional?> ObterPorIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Profissional>(
            "SELECT * FROM Profissionais WHERE Id=@Id AND Ativo=1", new { Id = id });
    }

    public async Task<IEnumerable<Profissional>> ListarPorSalaoAsync(int salaoId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Profissional>(
            @"SELECT p.*, u.Nome AS NomeUsuario, u.FotoUrl
              FROM Profissionais p
              INNER JOIN Usuarios u ON u.Id = p.UsuarioId
              WHERE p.SalaoId=@SalaoId AND p.Ativo=1",
            new { SalaoId = salaoId });
    }

    public async Task<int> CriarAsync(Profissional profissional)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Profissionais (UsuarioId,SalaoId,Especialidade,Biografia,NotaMedia,TotalAvaliacoes,Ativo,CriadoEm)
                    VALUES (@UsuarioId,@SalaoId,@Especialidade,@Biografia,@NotaMedia,@TotalAvaliacoes,@Ativo,@CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, profissional);
    }

    public async Task AtualizarAsync(Profissional profissional)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Profissionais SET Especialidade=@Especialidade,Biografia=@Biografia WHERE Id=@Id", profissional);
    }

    public async Task DeletarAsync(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Profissionais SET Ativo=0 WHERE Id=@Id", new { Id = id });
    }
}

// ── Servico ────────────────────────────────────────────────────────────────
public class ServicoRepository : IServicoRepository
{
    private readonly IDbConnectionFactory _db;
    public ServicoRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Servico?> ObterPorIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Servico>(
            "SELECT * FROM Servicos WHERE Id=@Id AND Ativo=1", new { Id = id });
    }

    public async Task<IEnumerable<Servico>> ListarPorSalaoAsync(int salaoId, int? categoriaId = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT s.*, c.Nome AS NomeCategoria FROM Servicos s INNER JOIN Categorias c ON c.Id=s.CategoriaId WHERE s.SalaoId=@SalaoId AND s.Ativo=1";
        if (categoriaId.HasValue) sql += " AND s.CategoriaId=@CategoriaId";
        sql += " ORDER BY s.Nome";
        return await conn.QueryAsync<Servico>(sql, new { SalaoId = salaoId, CategoriaId = categoriaId });
    }

    public async Task<int> CriarAsync(Servico servico)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Servicos (SalaoId,CategoriaId,Nome,Descricao,Preco,DuracaoMinutos,Ativo,CriadoEm)
                    VALUES (@SalaoId,@CategoriaId,@Nome,@Descricao,@Preco,@DuracaoMinutos,@Ativo,@CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, servico);
    }

    public async Task AtualizarAsync(Servico servico)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Servicos SET Nome=@Nome,Descricao=@Descricao,Preco=@Preco,DuracaoMinutos=@DuracaoMinutos,Ativo=@Ativo WHERE Id=@Id",
            servico);
    }

    public async Task DeletarAsync(int id)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync("UPDATE Servicos SET Ativo=0 WHERE Id=@Id", new { Id = id });
    }
}

// ── Agendamento ────────────────────────────────────────────────────────────
public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly IDbConnectionFactory _db;
    public AgendamentoRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Agendamento?> ObterPorIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Agendamento>(
            "SELECT * FROM Agendamentos WHERE Id=@Id", new { Id = id });
    }

    public async Task<IEnumerable<Agendamento>> ListarPorClienteAsync(int clienteId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Agendamento>(
            "SELECT * FROM Agendamentos WHERE ClienteId=@ClienteId ORDER BY DataHora DESC",
            new { ClienteId = clienteId });
    }

    public async Task<IEnumerable<Agendamento>> ListarPorProfissionalAsync(int profissionalId, DateTime? data = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Agendamentos WHERE ProfissionalId=@ProfissionalId";
        if (data.HasValue) sql += " AND CAST(DataHora AS DATE) = @Data";
        sql += " ORDER BY DataHora";
        return await conn.QueryAsync<Agendamento>(sql, new { ProfissionalId = profissionalId, Data = data?.Date });
    }

    public async Task<IEnumerable<Agendamento>> ListarPorSalaoAsync(int salaoId, DateTime? data = null)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Agendamentos WHERE SalaoId=@SalaoId";
        if (data.HasValue) sql += " AND CAST(DataHora AS DATE) = @Data";
        sql += " AND Status NOT IN (3) ORDER BY DataHora"; // Excluir cancelados
        return await conn.QueryAsync<Agendamento>(sql, new { SalaoId = salaoId, Data = data?.Date });
    }

    public async Task<bool> VerificarConflitoAsync(int profissionalId, DateTime dataHora, int duracaoMinutos, int? ignorarId = null)
    {
        using var conn = _db.CreateConnection();
        var fimNovo = dataHora.AddMinutes(duracaoMinutos);
        var sql = @"SELECT COUNT(1) FROM Agendamentos
                    WHERE ProfissionalId=@ProfissionalId
                    AND Status NOT IN (3, 5)
                    AND (
                        (@DataHora >= DataHora AND @DataHora < DATEADD(MINUTE, DuracaoMinutos, DataHora))
                        OR (@FimNovo > DataHora AND @FimNovo <= DATEADD(MINUTE, DuracaoMinutos, DataHora))
                        OR (DataHora >= @DataHora AND DataHora < @FimNovo)
                    )";
        if (ignorarId.HasValue) sql += " AND Id <> @IgnorarId";
        return await conn.QuerySingleAsync<int>(sql, new { ProfissionalId = profissionalId, DataHora = dataHora, FimNovo = fimNovo, IgnorarId = ignorarId }) > 0;
    }

    public async Task<int> CriarAsync(Agendamento agendamento)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Agendamentos (ClienteId,ProfissionalId,ServicoId,SalaoId,DataHora,
                    DuracaoMinutos,ValorTotal,Status,Observacoes,CriadoEm)
                    VALUES (@ClienteId,@ProfissionalId,@ServicoId,@SalaoId,@DataHora,
                    @DuracaoMinutos,@ValorTotal,@Status,@Observacoes,@CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, agendamento);
    }

    public async Task AtualizarStatusAsync(int id, StatusAgendamento status)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Agendamentos SET Status=@Status, AtualizadoEm=@AtualizadoEm WHERE Id=@Id",
            new { Id = id, Status = (int)status, AtualizadoEm = DateTime.UtcNow });
    }
}

// ── Avaliacao ──────────────────────────────────────────────────────────────
public class AvaliacaoRepository : IAvaliacaoRepository
{
    private readonly IDbConnectionFactory _db;
    public AvaliacaoRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Avaliacao>> ListarPorSalaoAsync(int salaoId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Avaliacao>(
            "SELECT * FROM Avaliacoes WHERE SalaoId=@SalaoId ORDER BY CriadoEm DESC",
            new { SalaoId = salaoId });
    }

    public async Task<IEnumerable<Avaliacao>> ListarPorProfissionalAsync(int profissionalId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Avaliacao>(
            "SELECT * FROM Avaliacoes WHERE ProfissionalId=@ProfissionalId ORDER BY CriadoEm DESC",
            new { ProfissionalId = profissionalId });
    }

    public async Task<bool> ExisteAvaliacaoParaAgendamentoAsync(int agendamentoId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QuerySingleAsync<int>(
            "SELECT COUNT(1) FROM Avaliacoes WHERE AgendamentoId=@AgendamentoId",
            new { AgendamentoId = agendamentoId }) > 0;
    }

    public async Task<int> CriarAsync(Avaliacao avaliacao)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Avaliacoes (AgendamentoId,ClienteId,ProfissionalId,SalaoId,Nota,Comentario,CriadoEm)
                    VALUES (@AgendamentoId,@ClienteId,@ProfissionalId,@SalaoId,@Nota,@Comentario,@CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                    -- Atualizar nota média do profissional
                    UPDATE Profissionais SET
                        NotaMedia = (SELECT AVG(CAST(Nota AS DECIMAL(3,2))) FROM Avaliacoes WHERE ProfissionalId=@ProfissionalId),
                        TotalAvaliacoes = (SELECT COUNT(1) FROM Avaliacoes WHERE ProfissionalId=@ProfissionalId)
                    WHERE Id=@ProfissionalId;";
        return await conn.QuerySingleAsync<int>(sql, avaliacao);
    }
}

// ── Plano ──────────────────────────────────────────────────────────────────
public class PlanoRepository : IPlanoRepository
{
    private readonly IDbConnectionFactory _db;
    public PlanoRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Domain.Entities.Plano>> ListarAtivosAsync()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Domain.Entities.Plano>(
            "SELECT * FROM Planos WHERE Ativo=1 ORDER BY Preco");
    }

    public async Task<Domain.Entities.Plano?> ObterPorIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Domain.Entities.Plano>(
            "SELECT * FROM Planos WHERE Id=@Id AND Ativo=1", new { Id = id });
    }
}

// ── Assinatura ─────────────────────────────────────────────────────────────
public class AssinaturaRepository : IAssinaturaRepository
{
    private readonly IDbConnectionFactory _db;
    public AssinaturaRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Domain.Entities.Assinatura?> ObterAtivaDoSalaoAsync(int salaoId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Domain.Entities.Assinatura>(
            "SELECT * FROM Assinaturas WHERE SalaoId=@SalaoId AND Status=1",
            new { SalaoId = salaoId });
    }

    public async Task<int> CriarAsync(Domain.Entities.Assinatura assinatura)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Assinaturas (SalaoId,PlanoId,Status,DataInicio,DataFim,CriadoEm)
                    VALUES (@SalaoId,@PlanoId,@Status,@DataInicio,@DataFim,@CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, assinatura);
    }

    public async Task AtualizarAsync(Domain.Entities.Assinatura assinatura)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Assinaturas SET Status=@Status WHERE Id=@Id", assinatura);
    }
}

// ── Notificacao ────────────────────────────────────────────────────────────
public class NotificacaoRepository : INotificacaoRepository
{
    private readonly IDbConnectionFactory _db;
    public NotificacaoRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Domain.Entities.Notificacao>> ListarPorUsuarioAsync(int usuarioId, bool apenasNaoLidas = false)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM Notificacoes WHERE UsuarioId=@UsuarioId";
        if (apenasNaoLidas) sql += " AND Lida=0";
        sql += " ORDER BY CriadoEm DESC";
        return await conn.QueryAsync<Domain.Entities.Notificacao>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<int> CriarAsync(Domain.Entities.Notificacao notificacao)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Notificacoes (UsuarioId,Titulo,Mensagem,Tipo,Lida,ReferenciaId,CriadoEm)
                    VALUES (@UsuarioId,@Titulo,@Mensagem,@Tipo,@Lida,@ReferenciaId,@CriadoEm);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, notificacao);
    }

    public async Task MarcarComoLidaAsync(int id, int usuarioId)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Notificacoes SET Lida=1 WHERE Id=@Id AND UsuarioId=@UsuarioId",
            new { Id = id, UsuarioId = usuarioId });
    }

    public async Task MarcarTodasComoLidasAsync(int usuarioId)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Notificacoes SET Lida=1 WHERE UsuarioId=@UsuarioId",
            new { UsuarioId = usuarioId });
    }
}

// ── Categoria ──────────────────────────────────────────────────────────────
public class CategoriaRepository : ICategoriaRepository
{
    private readonly IDbConnectionFactory _db;
    public CategoriaRepository(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Categoria>> ListarAsync()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Categoria>("SELECT * FROM Categorias WHERE Ativo=1 ORDER BY Nome");
    }

    public async Task<Categoria?> ObterPorIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Categoria>(
            "SELECT * FROM Categorias WHERE Id=@Id AND Ativo=1", new { Id = id });
    }
}
