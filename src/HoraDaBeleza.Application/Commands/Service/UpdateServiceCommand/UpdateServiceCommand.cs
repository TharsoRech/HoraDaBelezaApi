using MediatR;

namespace HoraDaBeleza.Application.Commands.Service.UpdateServiceCommand;

public record UpdateServiceCommand(int Id, int SalonId, int OwnerId, string Name,
    string? Description, decimal Price, int DurationMinutes, bool Active) : IRequest<Unit>;
