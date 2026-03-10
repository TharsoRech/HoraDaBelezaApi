using MediatR;

namespace HoraDaBeleza.Application.Commands.Salons.DeleteSalonCommand;

public record DeleteSalonCommand(int Id, int OwnerId) : IRequest<Unit>;
