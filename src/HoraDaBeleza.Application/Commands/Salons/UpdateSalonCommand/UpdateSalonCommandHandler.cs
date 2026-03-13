using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Salons.UpdateSalonCommand;

public class UpdateSalonCommandHandler(ISalonRepository repo) : IRequestHandler<UpdateSalonCommand, SalonDto>
{

    public async Task<SalonDto> Handle(Salons.UpdateSalonCommand.UpdateSalonCommand req, CancellationToken ct)
    {
        var salon = await repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Salon", req.Id);

        if (salon.OwnerId != req.OwnerId)
            throw new UnauthorizedException("You do not have permission to edit this salon.");

        salon.Name          = req.Name;
        salon.Description   = req.Description;
        salon.Address       = req.Address;
        salon.City          = req.City;
        salon.State         = req.State;
        salon.ZipCode       = req.ZipCode;
        salon.Latitude      = req.Latitude;
        salon.Longitude     = req.Longitude;
        salon.Phone         = req.Phone;
        salon.Email         = req.Email;
        salon.BusinessHours = req.BusinessHours;
        salon.UpdatedAt     = DateTime.UtcNow;

        await repo.UpdateAsync(salon);
        return new SalonDto(salon.Id, salon.OwnerId, salon.Name, salon.Description, salon.LogoUrl,
            salon.Address, salon.City, salon.State, salon.Phone,
            salon.Latitude, salon.Longitude, null, salon.Active, null, 0, null, null, false, false, false);
    }
}
