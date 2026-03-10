using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Service.CreateServiceCommand;

public class CreateServiceCommandHandler(IServiceRepository repo, ISalonRepository salonRepo, ICategoryRepository categoryRepo) : IRequestHandler<CreateServiceCommand, ServiceDto>
{

    public async Task<ServiceDto> Handle(CreateServiceCommand req, CancellationToken ct)
    {
        var salon = await salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
        if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

        var category = await categoryRepo.GetByIdAsync(req.CategoryId) ?? throw new NotFoundException("Category", req.CategoryId);

        var service = new Domain.Entities.Service
        {
            SalonId         = req.SalonId,
            CategoryId      = req.CategoryId,
            Name            = req.Name,
            Description     = req.Description,
            Price           = req.Price,
            DurationMinutes = req.DurationMinutes
        };

        var id = await repo.CreateAsync(service);
        return new ServiceDto(id, req.SalonId, req.CategoryId, category.Name,
            req.Name, req.Description, req.Price, req.DurationMinutes, true);
    }
}
