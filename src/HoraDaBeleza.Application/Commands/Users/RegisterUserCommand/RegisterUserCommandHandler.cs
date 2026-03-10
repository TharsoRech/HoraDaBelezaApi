using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users.RegisterUserCommand;

public class RegisterUserCommandHandler(IUserRepository repo) : IRequestHandler<RegisterUserCommand, UserDto>
{

    public async Task<UserDto> Handle(Users.RegisterUserCommand.RegisterUserCommand request, CancellationToken ct)
    {
        if (await repo.EmailExistsAsync(request.Email))
            throw new BusinessException("Email address is already registered.");

        var user = new User
        {
            Name         = request.Name,
            Email        = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone        = request.Phone,
            Type         = request.Type
        };

        user.Id = await repo.CreateAsync(user);
        return new UserDto(user.Id, user.Name, user.Email, user.Phone, user.PhotoUrl, user.Type, user.Active);
    }
}
