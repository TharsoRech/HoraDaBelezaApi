using System.Text;
using HoraDaBeleza.API.Middleware;
using HoraDaBeleza.Application;
using HoraDaBeleza.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Application & Infrastructure ──────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// ── JWT ────────────────────────────────────────────────────────────────────
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwt["Issuer"],
            ValidAudience            = jwt["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// ── Swagger ────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Hora da Beleza API",
        Version     = "v1",
        Description = """
            REST API for the **Hora da Beleza** beauty salon SaaS platform.

            ## User Types
            | Value | Type         | Permissions                           |
            |-------|--------------|---------------------------------------|
            | 1     | Client       | Book appointments, submit reviews     |
            | 2     | Professional | Manage schedule, update status        |
            | 3     | Owner        | Manage salon, services, professionals |
            | 4     | Admin        | Full access                           |

            ## Authentication
            1. `POST /api/auth/login` — get your token
            2. Click **Authorize 🔒** and enter: `Bearer {token}`
            """
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter: **Bearer {your token}**"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        },
        Array.Empty<string>()
    }});

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ── CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Pipeline ───────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hora da Beleza API v1");
    c.RoutePrefix = "swagger";   // Swagger at http://localhost:5000
    c.EnableFilter();
    c.EnableDeepLinking();
    c.DocumentTitle          = "Hora da Beleza API";
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
