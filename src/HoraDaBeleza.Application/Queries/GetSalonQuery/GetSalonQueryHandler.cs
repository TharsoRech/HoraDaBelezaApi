using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Queries.GetSalonQuery;

public class GetSalonQueryHandler : IRequestHandler<GetSalonQuery, SalonDto>
{
    private readonly ISalonRepository _repo;
    public GetSalonQueryHandler(ISalonRepository repo) => _repo = repo;

    public async Task<SalonDto> Handle(GetSalonQuery req, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Salon", req.Id);
        return new SalonDto(s.Id, s.OwnerId, s.Name, s.Description, s.LogoUrl,
            s.Address, s.City, s.State, s.Phone, s.Latitude, s.Longitude, null, s.Active);
    }
}
