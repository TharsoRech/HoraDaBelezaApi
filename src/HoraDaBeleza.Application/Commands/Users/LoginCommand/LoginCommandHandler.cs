using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users.LoginCommand;

public class LoginCommandHandler(IUserRepository repo, ITokenService token) : IRequestHandler<LoginCommand, LoginResponse>
{

    public async Task<LoginResponse> Handle(Users.LoginCommand.LoginCommand request, CancellationToken ct)
    {
        var user = await repo.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.Active)
            throw new BusinessException("Account is inactive. Please contact support.");

        return new LoginResponse(token.GenerateToken(user), user.Name, user.Email, user.Type);
    }
}
