using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Professional.CreateProfessionalCommand;

public class CreateProfessionalCommandHandler(IProfessionalRepository repo, ISalonRepository salonRepo, IUserRepository userRepo) : IRequestHandler<CreateProfessionalCommand, ProfessionalDto>
{

    public async Task<ProfessionalDto> Handle(CreateProfessionalCommand req, CancellationToken ct)
    {
        var salon = await salonRepo.GetByIdAsync(req.SalonId) ?? throw new NotFoundException("Salon", req.SalonId);
        if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();

        var user = await userRepo.GetByIdAsync(req.UserId) ?? throw new NotFoundException("User", req.UserId);

        var professional = new Domain.Entities.Professional
        {
            UserId    = req.UserId,
            SalonId   = req.SalonId,
            Specialty = req.Specialty,
            Bio       = req.Bio
        };

        var id = await repo.CreateAsync(professional);
        return new ProfessionalDto(id, req.UserId, req.SalonId, user.Name,
            user.Base64Image, req.Specialty, req.Bio, null, 0, true, null, null, false);
    }
}
