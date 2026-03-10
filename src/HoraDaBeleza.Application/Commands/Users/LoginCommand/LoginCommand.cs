using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users.LoginCommand;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
