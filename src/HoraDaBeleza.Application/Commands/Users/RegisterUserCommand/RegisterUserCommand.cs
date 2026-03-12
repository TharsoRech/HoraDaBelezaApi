using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Domain.Enums;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users.RegisterUserCommand;

public record RegisterUserCommand(string Name, string Email, string Password,
    string? Phone, UserType Type, string? Doc = null, string? Dob = null, string? Base64Image = null) : IRequest<UserDto>;
