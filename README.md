# 💈 Hora da Beleza — Backend API

Backend do aplicativo **Hora da Beleza** em **.NET 8**, seguindo arquitetura **Monolito** com padrões **CQRS + MediatR**, acesso a dados com **Dapper** e banco **SQL Server**.

---

## 🏗️ Arquitetura

```
HoraDaBeleza/
├── src/
│   ├── HoraDaBeleza.Domain          # Entidades, Enums, Exceptions
│   ├── HoraDaBeleza.Application     # CQRS Commands/Queries, DTOs, Interfaces, Validações
│   ├── HoraDaBeleza.Infrastructure  # Repositories (Dapper), JWT Service, DI
│   └── HoraDaBeleza.API             # Controllers, Middleware, Program.cs
└── Database/
    └── 001_CreateDatabase.sql       # Script completo de criação do banco
```

### Padrões utilizados
| Padrão | Implementação |
|--------|--------------|
| CQRS | MediatR — Commands (escrita) e Queries (leitura) separados |
| Repository Pattern | Interfaces em Application, implementações com Dapper em Infrastructure |
| Pipeline Behavior | Validação automática com FluentValidation antes de cada Command |
| Global Exception Handling | Middleware captura todas as exceções e retorna JSON padronizado |

---

## 🚀 Como rodar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local, Docker ou Azure)

### 1. Banco de dados
Execute o script no SQL Server Management Studio ou Azure Data Studio:
```
Database/001_CreateDatabase.sql
```

### 2. Connection string
Edite `src/HoraDaBeleza.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVER;Database=HoraDaBeleza;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "TROQUE_ESTA_CHAVE_POR_UMA_SEGURA_EM_PRODUCAO_MINIMO_32_CHARS"
  }
}
```

### 3. Rodar a API
```bash
cd src/HoraDaBeleza.API
dotnet run
```

Acesse o Swagger em: **http://localhost:5000** (ou porta configurada)

### SQL Server via Docker (opcional)
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SuaSenhaAqui@123" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## 🔐 Autenticação JWT

### Fluxo
1. `POST /api/auth/registrar` — criar conta
2. `POST /api/auth/login` — obter token JWT
3. Usar o token no header: `Authorization: Bearer {token}`

### Tipos de usuário
| Tipo | Acesso |
|------|--------|
| `Cliente` | Agendar, avaliar, ver histórico |
| `Profissional` | Ver agenda própria, confirmar/concluir |
| `Proprietario` | Gerenciar salão, serviços, profissionais |
| `Admin` | Acesso total |

---

## 📋 Endpoints principais

### Auth
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/auth/login` | ❌ | Login — retorna JWT |
| POST | `/api/auth/registrar` | ❌ | Registro de usuário |
| GET | `/api/auth/me` | ✅ | Perfil do usuário logado |
| PUT | `/api/auth/me` | ✅ | Atualizar perfil |

### Salões
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/saloes` | ❌ | Listar (filtros: cidade, busca) |
| GET | `/api/saloes/{id}` | ❌ | Detalhe do salão |
| GET | `/api/saloes/meus` | ✅ | Meus salões (proprietário) |
| POST | `/api/saloes` | ✅ | Criar salão |
| PUT | `/api/saloes/{id}` | ✅ | Atualizar salão |
| DELETE | `/api/saloes/{id}` | ✅ | Desativar salão |

### Serviços
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/saloes/{salaoId}/servicos` | ❌ | Listar serviços do salão |
| POST | `/api/saloes/{salaoId}/servicos` | ✅ | Criar serviço |
| PUT | `/api/saloes/{salaoId}/servicos/{id}` | ✅ | Atualizar |
| DELETE | `/api/saloes/{salaoId}/servicos/{id}` | ✅ | Deletar |

### Profissionais
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/saloes/{salaoId}/profissionais` | ❌ | Listar profissionais |
| POST | `/api/saloes/{salaoId}/profissionais` | ✅ | Vincular profissional |

### Agendamentos
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/agendamentos/meus` | ✅ | Meus agendamentos (cliente) |
| GET | `/api/agendamentos/salao/{salaoId}` | ✅ | Agenda do salão |
| GET | `/api/agendamentos/profissional/{id}` | ✅ | Agenda do profissional |
| POST | `/api/agendamentos` | ✅ | Criar agendamento |
| DELETE | `/api/agendamentos/{id}` | ✅ | Cancelar |
| PATCH | `/api/agendamentos/{id}/status` | ✅ | Atualizar status |

### Avaliações
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/avaliacoes/salao/{salaoId}` | ❌ | Avaliações do salão |
| POST | `/api/avaliacoes` | ✅ | Criar avaliação |

### Planos & Assinaturas
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/planos` | ❌ | Listar planos disponíveis |
| POST | `/api/assinaturas` | ✅ | Assinar plano |

### Notificações
| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| GET | `/api/notificacoes` | ✅ | Listar notificações |
| PATCH | `/api/notificacoes/{id}/lida` | ✅ | Marcar como lida |
| PATCH | `/api/notificacoes/lidas` | ✅ | Marcar todas como lidas |

---

## 📦 Pacotes NuGet

| Pacote | Versão | Uso |
|--------|--------|-----|
| MediatR | 12.2.0 | CQRS pipeline |
| Dapper | 2.1.35 | ORM leve sobre ADO.NET |
| Microsoft.Data.SqlClient | 5.2.1 | Driver SQL Server |
| FluentValidation | 11.9.0 | Validação de Commands |
| BCrypt.Net-Next | 4.0.3 | Hash de senhas |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.0 | JWT Auth |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger/OpenAPI |

---

## 🔧 Como fazer push para o GitHub

```bash
# Na raiz do projeto
git init
git add .
git commit -m "feat: backend inicial com CQRS, Dapper, JWT e SQL Server"
git remote add origin https://github.com/SEU_USUARIO/horadabeleza-api.git
git push -u origin main
```
