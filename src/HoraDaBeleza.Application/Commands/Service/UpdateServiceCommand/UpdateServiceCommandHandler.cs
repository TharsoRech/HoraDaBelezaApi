using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Service.UpdateServiceCommand;

public class UpdateServiceCommandHandler(IServiceRepository repo, ISalonRepository salonRepo) : IRequestHandler<UpdateServiceCommand, Unit>
{

    public async Task<Unit> Handle(UpdateServiceCommand req, CancellationToken ct)
    {
        var salon = await salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
        if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

        var service = await repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Service", req.Id);
        service.Name            = req.Name;
        service.Description     = req.Description;
        service.Price           = req.Price;
        service.DurationMinutes = req.DurationMinutes;
        service.Active          = req.Active;

        await repo.UpdateAsync(service);
        return Unit.Value;
    }
}
