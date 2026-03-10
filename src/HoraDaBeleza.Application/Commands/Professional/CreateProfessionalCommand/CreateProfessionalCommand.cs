using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Professional.CreateProfessionalCommand;

public record CreateProfessionalCommand(int UserId, int SalonId, int OwnerId,
    string? Specialty, string? Bio) : IRequest<ProfessionalDto>;
