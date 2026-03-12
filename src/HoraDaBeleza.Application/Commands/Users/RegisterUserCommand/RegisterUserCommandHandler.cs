using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users.RegisterUserCommand;

public class RegisterUserCommandHandler(IUserRepository repo) : IRequestHandler<RegisterUserCommand, UserDto>
{

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await repo.EmailExistsAsync(request.Email))
            throw new BusinessException("Email address is already registered.");

        // Validação de CPF/CNPJ duplicado
        if (!string.IsNullOrEmpty(request.Doc))
        {
            if (await repo.DocExistsAsync(request.Doc))
                throw new BusinessException("Já existe um usuário cadastrado com este CPF/CNPJ.");
        }

        var user = new User
        {
            Name         = request.Name,
            Email        = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone        = request.Phone,
            Doc          = request.Doc, // Salva o CPF/CNPJ
            Dob          = request.Dob != null ? DateTime.Parse(request.Dob) : null, // Salva a data de nascimento
            Base64Image  = request.Base64Image, // Salva a imagem em base64
            Type         = request.Type
        };

        user.Id = await repo.CreateAsync(user);
        return new UserDto(user.Id, user.Name, user.Email, user.Phone,  user.Base64Image, user.Type, user.Active, user.Doc, user.Dob?.ToString("yyyy-MM-dd"));
    }
}
