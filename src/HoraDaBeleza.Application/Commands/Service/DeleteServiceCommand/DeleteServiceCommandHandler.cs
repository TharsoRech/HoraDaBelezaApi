using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Service.DeleteServiceCommand;

public class DeleteServiceCommandHandler(IServiceRepository repo, ISalonRepository salonRepo) : IRequestHandler<DeleteServiceCommand, Unit>
{

    public async Task<Unit> Handle(DeleteServiceCommand req, CancellationToken ct)
    {
        var salon = await salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
        if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();
        await repo.DeleteAsync(req.Id);
        return Unit.Value;
    }
}
