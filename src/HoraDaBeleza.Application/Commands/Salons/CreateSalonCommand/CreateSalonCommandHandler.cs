using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Salons.CreateSalonCommand;

public class CreateSalonCommandHandler(ISalonRepository repo) : IRequestHandler<CreateSalonCommand, SalonDto>
{

    public async Task<SalonDto> Handle(Salons.CreateSalonCommand.CreateSalonCommand req, CancellationToken ct)
    {
        var salon = new Salon
        {
            OwnerId       = req.OwnerId,
            Name          = req.Name,
            Description   = req.Description,
            Address       = req.Address,
            City          = req.City,
            State         = req.State,
            ZipCode       = req.ZipCode,
            Latitude      = req.Latitude,
            Longitude     = req.Longitude,
            Phone         = req.Phone,
            Email         = req.Email,
            BusinessHours = req.BusinessHours
        };

        salon.Id = await repo.CreateAsync(salon);
        return ToDto(salon);
    }

    private static SalonDto ToDto(Salon s) =>
        new(s.Id, s.OwnerId, s.Name, s.Description, s.LogoUrl,
            s.Address, s.City, s.State, s.Phone, s.Latitude, s.Longitude, null, s.Active);
}
