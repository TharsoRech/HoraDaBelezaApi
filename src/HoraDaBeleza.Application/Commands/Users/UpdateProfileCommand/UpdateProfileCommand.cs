using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Users.UpdateProfileCommand;

public record UpdateProfileCommand(int UserId, string Name, string? Phone, string? PhotoUrl, string? Base64Image) : IRequest<UserDto>;
