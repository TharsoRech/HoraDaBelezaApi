using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Salons;

// ── Create ─────────────────────────────────────────────────────────────────
public record CreateSalonCommand(int OwnerId, string Name, string? Description,
    string Address, string City, string State, string? ZipCode,
    decimal? Latitude, decimal? Longitude, string? Phone,
    string? Email, string? BusinessHours) : IRequest<SalonDto>;

public class CreateSalonCommandValidator : AbstractValidator<CreateSalonCommand>
{
    public CreateSalonCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty().Length(2);
    }
}

public class CreateSalonCommandHandler : IRequestHandler<CreateSalonCommand, SalonDto>
{
    private readonly ISalonRepository _repo;
    public CreateSalonCommandHandler(ISalonRepository repo) => _repo = repo;

    public async Task<SalonDto> Handle(CreateSalonCommand req, CancellationToken ct)
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

        salon.Id = await _repo.CreateAsync(salon);
        return ToDto(salon);
    }

    private static SalonDto ToDto(Salon s) =>
        new(s.Id, s.OwnerId, s.Name, s.Description, s.LogoUrl,
            s.Address, s.City, s.State, s.Phone, s.Latitude, s.Longitude, null, s.Active);
}

// ── Update ─────────────────────────────────────────────────────────────────
public record UpdateSalonCommand(int Id, int OwnerId, string Name, string? Description,
    string Address, string City, string State, string? ZipCode,
    decimal? Latitude, decimal? Longitude, string? Phone,
    string? Email, string? BusinessHours) : IRequest<SalonDto>;

public class UpdateSalonCommandHandler : IRequestHandler<UpdateSalonCommand, SalonDto>
{
    private readonly ISalonRepository _repo;
    public UpdateSalonCommandHandler(ISalonRepository repo) => _repo = repo;

    public async Task<SalonDto> Handle(UpdateSalonCommand req, CancellationToken ct)
    {
        var salon = await _repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Salon", req.Id);

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

        await _repo.UpdateAsync(salon);
        return new SalonDto(salon.Id, salon.OwnerId, salon.Name, salon.Description, salon.LogoUrl,
            salon.Address, salon.City, salon.State, salon.Phone,
            salon.Latitude, salon.Longitude, null, salon.Active);
    }
}

// ── Delete ─────────────────────────────────────────────────────────────────
public record DeleteSalonCommand(int Id, int OwnerId) : IRequest<Unit>;

public class DeleteSalonCommandHandler : IRequestHandler<DeleteSalonCommand, Unit>
{
    private readonly ISalonRepository _repo;
    public DeleteSalonCommandHandler(ISalonRepository repo) => _repo = repo;

    public async Task<Unit> Handle(DeleteSalonCommand req, CancellationToken ct)
    {
        var salon = await _repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Salon", req.Id);
        if (salon.OwnerId != req.OwnerId) throw new UnauthorizedException();
        await _repo.DeleteAsync(req.Id);
        return Unit.Value;
    }
}
