using MediatR;

namespace HoraDaBeleza.Application.Commands.Service.DeleteServiceCommand;

public record DeleteServiceCommand(int Id, int SalonId, int OwnerId) : IRequest<Unit>;
