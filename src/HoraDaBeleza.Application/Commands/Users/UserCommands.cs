using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users;

// ── Login ──────────────────────────────────────────────────────────────────
public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _repo;
    private readonly ITokenService   _token;

    public LoginCommandHandler(IUserRepository repo, ITokenService token)
    {
        _repo  = repo;
        _token = token;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.Active)
            throw new BusinessException("Account is inactive. Please contact support.");

        return new LoginResponse(_token.GenerateToken(user), user.Name, user.Email, user.Type);
    }
}

// ── Register ───────────────────────────────────────────────────────────────
public record RegisterUserCommand(string Name, string Email, string Password,
    string? Phone, UserType Type) : IRequest<UserDto>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _repo;
    public RegisterUserCommandHandler(IUserRepository repo) => _repo = repo;

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await _repo.EmailExistsAsync(request.Email))
            throw new BusinessException("Email address is already registered.");

        var user = new User
        {
            Name         = request.Name,
            Email        = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone        = request.Phone,
            Type         = request.Type
        };

        user.Id = await _repo.CreateAsync(user);
        return new UserDto(user.Id, user.Name, user.Email, user.Phone, user.PhotoUrl, user.Type, user.Active);
    }
}

// ── Update Profile ─────────────────────────────────────────────────────────
public record UpdateProfileCommand(int UserId, string Name, string? Phone, string? PhotoUrl) : IRequest<UserDto>;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly IUserRepository _repo;
    public UpdateProfileCommandHandler(IUserRepository repo) => _repo = repo;

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException("User", request.UserId);

        user.Name      = request.Name;
        user.Phone     = request.Phone;
        user.PhotoUrl  = request.PhotoUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(user);
        return new UserDto(user.Id, user.Name, user.Email, user.Phone, user.PhotoUrl, user.Type, user.Active);
    }
}
