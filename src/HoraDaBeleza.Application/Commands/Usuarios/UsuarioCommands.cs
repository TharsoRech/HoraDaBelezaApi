using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Usuarios;

// ── Login ──────────────────────────────────────────────────────────────────
public record LoginCommand(string Email, string Senha) : IRequest<LoginResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Senha).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUsuarioRepository _repo;
    private readonly ITokenService _token;

    public LoginCommandHandler(IUsuarioRepository repo, ITokenService token)
    {
        _repo = repo;
        _token = token;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var usuario = await _repo.ObterPorEmailAsync(request.Email)
            ?? throw new UnauthorizedException("Email ou senha inválidos.");

        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            throw new UnauthorizedException("Email ou senha inválidos.");

        if (!usuario.Ativo)
            throw new BusinessException("Usuário inativo. Entre em contato com o suporte.");

        var token = _token.GerarToken(usuario);
        return new LoginResponse(token, usuario.Nome, usuario.Email, usuario.Tipo);
    }
}

// ── Registro ───────────────────────────────────────────────────────────────
public record RegistrarUsuarioCommand(string Nome, string Email, string Senha,
    string? Telefone, TipoUsuario Tipo) : IRequest<UsuarioDto>;

public class RegistrarUsuarioCommandValidator : AbstractValidator<RegistrarUsuarioCommand>
{
    public RegistrarUsuarioCommandValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6);
    }
}

public class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, UsuarioDto>
{
    private readonly IUsuarioRepository _repo;

    public RegistrarUsuarioCommandHandler(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<UsuarioDto> Handle(RegistrarUsuarioCommand request, CancellationToken ct)
    {
        if (await _repo.ExisteEmailAsync(request.Email))
            throw new BusinessException("Email já cadastrado.");

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email.ToLower().Trim(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Telefone = request.Telefone,
            Tipo = request.Tipo
        };

        var id = await _repo.CriarAsync(usuario);
        usuario.Id = id;

        return new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email,
            usuario.Telefone, usuario.FotoUrl, usuario.Tipo, usuario.Ativo);
    }
}

// ── Atualizar Perfil ────────────────────────────────────────────────────────
public record AtualizarPerfilCommand(int UsuarioId, string Nome, string? Telefone, string? FotoUrl) : IRequest<UsuarioDto>;

public class AtualizarPerfilCommandHandler : IRequestHandler<AtualizarPerfilCommand, UsuarioDto>
{
    private readonly IUsuarioRepository _repo;

    public AtualizarPerfilCommandHandler(IUsuarioRepository repo) => _repo = repo;

    public async Task<UsuarioDto> Handle(AtualizarPerfilCommand request, CancellationToken ct)
    {
        var usuario = await _repo.ObterPorIdAsync(request.UsuarioId)
            ?? throw new NotFoundException("Usuário", request.UsuarioId);

        usuario.Nome = request.Nome;
        usuario.Telefone = request.Telefone;
        usuario.FotoUrl = request.FotoUrl;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _repo.AtualizarAsync(usuario);

        return new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email,
            usuario.Telefone, usuario.FotoUrl, usuario.Tipo, usuario.Ativo);
    }
}
