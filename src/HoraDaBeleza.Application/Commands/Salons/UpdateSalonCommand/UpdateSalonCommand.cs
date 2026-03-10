using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Salons.UpdateSalonCommand;

public record UpdateSalonCommand(int Id, int OwnerId, string Name, string? Description,
    string Address, string City, string State, string? ZipCode,
    decimal? Latitude, decimal? Longitude, string? Phone,
    string? Email, string? BusinessHours) : IRequest<SalonDto>;
