-- ============================================================
--  Hora da Beleza — Script de criação do banco SQL Server
--  Executar na ordem: cria o banco, depois todas as tabelas
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HoraDaBeleza')
    CREATE DATABASE HoraDaBeleza;
GO

USE HoraDaBeleza;
GO

-- ── Categorias ────────────────────────────────────────────────────────────
CREATE TABLE Categorias (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Nome     NVARCHAR(100) NOT NULL,
    IconeUrl NVARCHAR(500) NULL,
    Ativo    BIT NOT NULL DEFAULT 1
);
GO

-- ── Usuários ──────────────────────────────────────────────────────────────
CREATE TABLE Usuarios (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Nome        NVARCHAR(100)  NOT NULL,
    Email       NVARCHAR(200)  NOT NULL UNIQUE,
    SenhaHash   NVARCHAR(500)  NOT NULL,
    Telefone    NVARCHAR(20)   NULL,
    FotoUrl     NVARCHAR(500)  NULL,
    -- 1=Cliente 2=Profissional 3=Proprietario 4=Admin
    Tipo        TINYINT        NOT NULL DEFAULT 1,
    Ativo       BIT            NOT NULL DEFAULT 1,
    CriadoEm   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    AtualizadoEm DATETIME2    NULL
);
GO

CREATE INDEX IX_Usuarios_Email ON Usuarios(Email);
GO

-- ── Planos de assinatura ──────────────────────────────────────────────────
CREATE TABLE Planos (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    Nome                NVARCHAR(100) NOT NULL,
    Descricao           NVARCHAR(500) NULL,
    Preco               DECIMAL(10,2) NOT NULL,
    PeriodoDias         INT           NOT NULL,
    LimiteAgendamentos  INT           NOT NULL DEFAULT 0, -- 0 = ilimitado
    Ativo               BIT           NOT NULL DEFAULT 1
);
GO

-- ── Salões ────────────────────────────────────────────────────────────────
CREATE TABLE Saloes (
    Id                    INT IDENTITY(1,1) PRIMARY KEY,
    ProprietarioId        INT            NOT NULL REFERENCES Usuarios(Id),
    Nome                  NVARCHAR(150)  NOT NULL,
    Descricao             NVARCHAR(1000) NULL,
    LogoUrl               NVARCHAR(500)  NULL,
    Endereco              NVARCHAR(300)  NOT NULL,
    Cidade                NVARCHAR(100)  NOT NULL,
    Estado                CHAR(2)        NOT NULL,
    Cep                   CHAR(9)        NULL,
    Latitude              DECIMAL(10,8)  NULL,
    Longitude             DECIMAL(11,8)  NULL,
    Telefone              NVARCHAR(20)   NULL,
    Email                 NVARCHAR(200)  NULL,
    HorarioFuncionamento  NVARCHAR(500)  NULL,
    Ativo                 BIT            NOT NULL DEFAULT 1,
    CriadoEm             DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    AtualizadoEm         DATETIME2      NULL
);
GO

CREATE INDEX IX_Saloes_Cidade ON Saloes(Cidade);
CREATE INDEX IX_Saloes_ProprietarioId ON Saloes(ProprietarioId);
GO

-- ── Assinaturas ───────────────────────────────────────────────────────────
CREATE TABLE Assinaturas (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    SalaoId     INT       NOT NULL REFERENCES Saloes(Id),
    PlanoId     INT       NOT NULL REFERENCES Planos(Id),
    -- 1=Ativa 2=Cancelada 3=Expirada 4=Suspensa
    Status      TINYINT   NOT NULL DEFAULT 1,
    DataInicio  DATETIME2 NOT NULL,
    DataFim     DATETIME2 NOT NULL,
    CriadoEm   DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── Profissionais ─────────────────────────────────────────────────────────
CREATE TABLE Profissionais (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId        INT           NOT NULL REFERENCES Usuarios(Id),
    SalaoId          INT           NOT NULL REFERENCES Saloes(Id),
    Especialidade    NVARCHAR(200) NULL,
    Biografia        NVARCHAR(1000) NULL,
    NotaMedia        DECIMAL(3,2)  NULL,
    TotalAvaliacoes  INT           NOT NULL DEFAULT 0,
    Ativo            BIT           NOT NULL DEFAULT 1,
    CriadoEm        DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE INDEX IX_Profissionais_SalaoId ON Profissionais(SalaoId);
GO

-- ── Serviços ──────────────────────────────────────────────────────────────
CREATE TABLE Servicos (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    SalaoId          INT            NOT NULL REFERENCES Saloes(Id),
    CategoriaId      INT            NOT NULL REFERENCES Categorias(Id),
    Nome             NVARCHAR(100)  NOT NULL,
    Descricao        NVARCHAR(500)  NULL,
    Preco            DECIMAL(10,2)  NOT NULL,
    DuracaoMinutos   INT            NOT NULL,
    Ativo            BIT            NOT NULL DEFAULT 1,
    CriadoEm        DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE INDEX IX_Servicos_SalaoId ON Servicos(SalaoId);
CREATE INDEX IX_Servicos_CategoriaId ON Servicos(CategoriaId);
GO

-- ── Agendamentos ──────────────────────────────────────────────────────────
CREATE TABLE Agendamentos (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId        INT            NOT NULL REFERENCES Usuarios(Id),
    ProfissionalId   INT            NOT NULL REFERENCES Profissionais(Id),
    ServicoId        INT            NOT NULL REFERENCES Servicos(Id),
    SalaoId          INT            NOT NULL REFERENCES Saloes(Id),
    DataHora         DATETIME2      NOT NULL,
    DuracaoMinutos   INT            NOT NULL,
    ValorTotal       DECIMAL(10,2)  NOT NULL,
    -- 1=Pendente 2=Confirmado 3=Cancelado 4=Concluido 5=NaoCompareceu
    Status           TINYINT        NOT NULL DEFAULT 1,
    Observacoes      NVARCHAR(500)  NULL,
    CriadoEm        DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    AtualizadoEm    DATETIME2      NULL
);
GO

CREATE INDEX IX_Agendamentos_ClienteId       ON Agendamentos(ClienteId);
CREATE INDEX IX_Agendamentos_ProfissionalId  ON Agendamentos(ProfissionalId);
CREATE INDEX IX_Agendamentos_SalaoId         ON Agendamentos(SalaoId);
CREATE INDEX IX_Agendamentos_DataHora        ON Agendamentos(DataHora);
GO

-- ── Avaliações ────────────────────────────────────────────────────────────
CREATE TABLE Avaliacoes (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    AgendamentoId    INT           NOT NULL UNIQUE REFERENCES Agendamentos(Id),
    ClienteId        INT           NOT NULL REFERENCES Usuarios(Id),
    ProfissionalId   INT           NOT NULL REFERENCES Profissionais(Id),
    SalaoId          INT           NOT NULL REFERENCES Saloes(Id),
    Nota             TINYINT       NOT NULL CHECK (Nota BETWEEN 1 AND 5),
    Comentario       NVARCHAR(1000) NULL,
    CriadoEm        DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── Notificações ──────────────────────────────────────────────────────────
CREATE TABLE Notificacoes (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId    INT            NOT NULL REFERENCES Usuarios(Id),
    Titulo       NVARCHAR(200)  NOT NULL,
    Mensagem     NVARCHAR(1000) NOT NULL,
    -- 1=AgendConfirmado 2=AgendCancelado 3=Lembrete 4=NovaAvaliacao 5=Promocao 6=Sistema
    Tipo         TINYINT        NOT NULL DEFAULT 6,
    Lida         BIT            NOT NULL DEFAULT 0,
    ReferenciaId INT            NULL,
    CriadoEm   DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE INDEX IX_Notificacoes_UsuarioId ON Notificacoes(UsuarioId);
GO

-- ============================================================
--  SEED DATA — dados iniciais
-- ============================================================

-- Categorias
INSERT INTO Categorias (Nome, IconeUrl) VALUES
    ('Cabelo',       'https://cdn.exemplo.com/icons/cabelo.png'),
    ('Unhas',        'https://cdn.exemplo.com/icons/unhas.png'),
    ('Estética',     'https://cdn.exemplo.com/icons/estetica.png'),
    ('Maquiagem',    'https://cdn.exemplo.com/icons/maquiagem.png'),
    ('Barba',        'https://cdn.exemplo.com/icons/barba.png'),
    ('Massagem',     'https://cdn.exemplo.com/icons/massagem.png'),
    ('Sobrancelha',  'https://cdn.exemplo.com/icons/sobrancelha.png'),
    ('Depilação',    'https://cdn.exemplo.com/icons/depilacao.png');

-- Planos
INSERT INTO Planos (Nome, Descricao, Preco, PeriodoDias, LimiteAgendamentos) VALUES
    ('Básico',       'Até 50 agendamentos por mês',           49.90,  30,  50),
    ('Profissional', 'Agendamentos ilimitados + relatórios',  99.90,  30,  0),
    ('Anual',        'Plano Profissional com 2 meses grátis', 999.90, 365, 0);

-- Usuário Admin (senha: Admin@123)
INSERT INTO Usuarios (Nome, Email, SenhaHash, Tipo) VALUES
    ('Administrador', 'admin@horadabeleza.com',
     '$2a$11$example_hash_change_in_production', 4);

GO
PRINT 'Banco de dados HoraDaBeleza criado com sucesso!';
