using System.Reflection;
using System.Text;
using HoraDaBeleza.API.Middleware;
using HoraDaBeleza.Application;
using HoraDaBeleza.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Application + Infrastructure ──────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// ── JWT Authentication ─────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwtSettings["Issuer"],
        ValidAudience            = jwtSettings["Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(key),
        ClockSkew                = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── Controllers com suporte a XML comments ────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ──────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "💈 Hora da Beleza API",
        Version     = "v1",
        Description = """
            API REST do aplicativo **Hora da Beleza** — plataforma de agendamento de serviços de beleza.
            
            ## Autenticação
            1. Use `POST /api/auth/registrar` para criar sua conta.
            2. Use `POST /api/auth/login` para obter o token JWT.
            3. Clique em **Authorize** 🔒 e informe: `Bearer {seu_token}`.
            
            ## Tipos de usuário
            | Tipo | Permissões |
            |------|-----------|
            | `Cliente` | Agendar, avaliar, ver histórico |
            | `Profissional` | Gerenciar agenda, confirmar/concluir |
            | `Proprietario` | Gerenciar salão, serviços e profissionais |
            | `Admin` | Acesso total |
            """,
        Contact = new OpenApiContact
        {
            Name = "Tharso Rech",
            Url  = new Uri("https://github.com/TharsoRech/HoraDaBelezaApi")
        }
    });

    // ── Botão Authorize com suporte a Bearer ──────────────────────────────
    var securityScheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Description  = "Informe o token JWT no formato: **Bearer {token}**",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id   = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });

    // ── XML Comments (descrições dos endpoints) ───────────────────────────
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    // ── Ordenar endpoints por área/tag ────────────────────────────────────
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    options.DocInclusionPredicate((_, _) => true);
});

// ── CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ── Middleware Pipeline ────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

// Swagger disponível em todos os ambientes (ajuste se quiser só em Dev)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hora da Beleza API v1");
    c.RoutePrefix      = string.Empty;        // Swagger na raiz: http://localhost:5000
    c.DocumentTitle    = "💈 Hora da Beleza API";
    c.DefaultModelsExpandDepth(-1);           // Esconde schemas por padrão (UI mais limpa)
    c.DisplayRequestDuration();               // Mostra tempo de resposta
    c.EnableFilter();                         // Caixa de busca de endpoints
    c.EnableDeepLinking();                    // URLs navegáveis por endpoint
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
