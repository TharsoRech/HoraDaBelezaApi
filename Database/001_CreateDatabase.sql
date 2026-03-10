-- ============================================================
-- Hora da Beleza — Database Schema
-- SQL Server 2019+
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HoraDaBeleza')
    CREATE DATABASE HoraDaBeleza;
GO

USE HoraDaBeleza;
GO

-- ── Users ──────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(100)  NOT NULL,
    Email        NVARCHAR(200)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255)  NOT NULL,
    Phone        NVARCHAR(20)   NULL,
    PhotoUrl     NVARCHAR(500)  NULL,
    Type         INT            NOT NULL DEFAULT 1,  -- 1=Client 2=Professional 3=Owner 4=Admin
    Active       BIT            NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt    DATETIME2      NULL
);
GO

-- ── Categories ─────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
CREATE TABLE Categories (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Name    NVARCHAR(100) NOT NULL,
    IconUrl NVARCHAR(500) NULL,
    Active  BIT           NOT NULL DEFAULT 1
);
GO

-- ── Salons ─────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Salons' AND xtype='U')
CREATE TABLE Salons (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    OwnerId       INT            NOT NULL REFERENCES Users(Id),
    Name          NVARCHAR(150)  NOT NULL,
    Description   NVARCHAR(1000) NULL,
    LogoUrl       NVARCHAR(500)  NULL,
    Address       NVARCHAR(300)  NOT NULL,
    City          NVARCHAR(100)  NOT NULL,
    State         NVARCHAR(2)    NOT NULL,
    ZipCode       NVARCHAR(10)   NULL,
    Latitude      DECIMAL(10,8)  NULL,
    Longitude     DECIMAL(11,8)  NULL,
    Phone         NVARCHAR(20)   NULL,
    Email         NVARCHAR(200)  NULL,
    BusinessHours NVARCHAR(500)  NULL,
    Active        BIT            NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt     DATETIME2      NULL
);
GO

-- ── Professionals ──────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Professionals' AND xtype='U')
CREATE TABLE Professionals (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT            NOT NULL REFERENCES Users(Id),
    SalonId       INT            NOT NULL REFERENCES Salons(Id),
    Specialty     NVARCHAR(200)  NULL,
    Bio           NVARCHAR(1000) NULL,
    AverageRating DECIMAL(3,2)   NULL DEFAULT 0,
    TotalReviews  INT            NOT NULL DEFAULT 0,
    Active        BIT            NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── Services ───────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Services' AND xtype='U')
CREATE TABLE Services (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    SalonId         INT            NOT NULL REFERENCES Salons(Id),
    CategoryId      INT            NOT NULL REFERENCES Categories(Id),
    Name            NVARCHAR(100)  NOT NULL,
    Description     NVARCHAR(500)  NULL,
    Price           DECIMAL(10,2)  NOT NULL,
    DurationMinutes INT            NOT NULL,
    Active          BIT            NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── Appointments ───────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Appointments' AND xtype='U')
CREATE TABLE Appointments (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ClientId        INT            NOT NULL REFERENCES Users(Id),
    ProfessionalId  INT            NOT NULL REFERENCES Professionals(Id),
    ServiceId       INT            NOT NULL REFERENCES Services(Id),
    SalonId         INT            NOT NULL REFERENCES Salons(Id),
    ScheduledAt     DATETIME2      NOT NULL,
    DurationMinutes INT            NOT NULL,
    TotalPrice      DECIMAL(10,2)  NOT NULL,
    Status          INT            NOT NULL DEFAULT 1, -- 1=Pending 2=Confirmed 3=Cancelled 4=Completed 5=NoShow
    Notes           NVARCHAR(500)  NULL,
    CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2      NULL
);
GO

-- ── Reviews ────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Reviews' AND xtype='U')
CREATE TABLE Reviews (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId  INT            NOT NULL REFERENCES Appointments(Id),
    ClientId       INT            NOT NULL REFERENCES Users(Id),
    ProfessionalId INT            NOT NULL REFERENCES Professionals(Id),
    SalonId        INT            NOT NULL REFERENCES Salons(Id),
    Rating         INT            NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment        NVARCHAR(1000) NULL,
    CreatedAt      DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── Plans ──────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Plans' AND xtype='U')
CREATE TABLE Plans (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Name             NVARCHAR(100)  NOT NULL,
    Description      NVARCHAR(500)  NULL,
    Price            DECIMAL(10,2)  NOT NULL,
    PeriodDays       INT            NOT NULL,
    AppointmentLimit INT            NOT NULL DEFAULT 0, -- 0 = unlimited
    Active           BIT            NOT NULL DEFAULT 1
);
GO

-- ── Subscriptions ──────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Subscriptions' AND xtype='U')
CREATE TABLE Subscriptions (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    SalonId   INT       NOT NULL REFERENCES Salons(Id),
    PlanId    INT       NOT NULL REFERENCES Plans(Id),
    Status    INT       NOT NULL DEFAULT 1, -- 1=Active 2=Cancelled 3=Expired 4=Suspended
    StartDate DATETIME2 NOT NULL,
    EndDate   DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── Notifications ──────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' AND xtype='U')
CREATE TABLE Notifications (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT            NOT NULL REFERENCES Users(Id),
    Title       NVARCHAR(200)  NOT NULL,
    Message     NVARCHAR(1000) NOT NULL,
    Type        INT            NOT NULL, -- 1=Confirmed 2=Cancelled 3=Reminder 4=Review 5=Promotion 6=System
    [Read]      BIT            NOT NULL DEFAULT 0,
    ReferenceId INT            NULL,
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── Indexes ────────────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Appointments_ClientId')
    CREATE INDEX IX_Appointments_ClientId       ON Appointments(ClientId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Appointments_ProfessionalId')
    CREATE INDEX IX_Appointments_ProfessionalId ON Appointments(ProfessionalId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Appointments_SalonId')
    CREATE INDEX IX_Appointments_SalonId        ON Appointments(SalonId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Appointments_ScheduledAt')
    CREATE INDEX IX_Appointments_ScheduledAt    ON Appointments(ScheduledAt);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Notifications_UserId')
    CREATE INDEX IX_Notifications_UserId        ON Notifications(UserId);
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Salons_City')
    CREATE INDEX IX_Salons_City                 ON Salons(City);
GO

-- ── Seed Data ──────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM Categories)
INSERT INTO Categories (Name, IconUrl) VALUES
    ('Hair',       'https://cdn.example.com/icons/hair.png'),
    ('Nails',      'https://cdn.example.com/icons/nails.png'),
    ('Aesthetics', 'https://cdn.example.com/icons/aesthetics.png'),
    ('Barbershop', 'https://cdn.example.com/icons/barbershop.png'),
    ('Massage',    'https://cdn.example.com/icons/massage.png'),
    ('Makeup',     'https://cdn.example.com/icons/makeup.png'),
    ('Eyebrows',   'https://cdn.example.com/icons/eyebrows.png'),
    ('Eyelashes',  'https://cdn.example.com/icons/eyelashes.png');
GO

IF NOT EXISTS (SELECT 1 FROM Plans)
INSERT INTO Plans (Name, Description, Price, PeriodDays, AppointmentLimit) VALUES
    ('Starter',      '30-day trial plan',             0.00,  30,  50),
    ('Professional', 'Up to 200 appointments/month', 89.90,  30, 200),
    ('Business',     'Unlimited appointments',       199.90, 30,   0);
GO

-- Admin user  (password: Admin@123)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@horadabeleza.com')
INSERT INTO Users (Name, Email, PasswordHash, Type)
VALUES ('Admin', 'admin@horadabeleza.com',
        '$2a$11$rHj7xJz1K8mN2pQ9vL5uOu3YwXtE6kF0dA4bC7hI1jM8nO2pR3sT4', 4);
GO

PRINT 'Database created and seeded successfully!';
