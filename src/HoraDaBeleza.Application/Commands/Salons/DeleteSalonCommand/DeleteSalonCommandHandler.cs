using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Salons.DeleteSalonCommand;

public class DeleteSalonCommandHandler(ISalonRepository repo) : IRequestHandler<DeleteSalonCommand, Unit>
{

    public async Task<Unit> Handle(Salons.DeleteSalonCommand.DeleteSalonCommand req, CancellationToken ct)
    {
        var salon = await repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Salon", req.Id);
        if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();
        await repo.DeleteAsync(req.Id);
        return Unit.Value;
    }
}
