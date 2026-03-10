using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Service.CreateServiceCommand;

public record CreateServiceCommand(int SalonId, int OwnerId, int CategoryId, string Name,
    string? Description, decimal Price, int DurationMinutes) : IRequest<ServiceDto>;
