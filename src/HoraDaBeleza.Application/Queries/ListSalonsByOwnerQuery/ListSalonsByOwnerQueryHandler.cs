using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListSalonsByOwnerQuery;

public class ListSalonsByOwnerQueryHandler : IRequestHandler<ListSalonsByOwnerQuery, IEnumerable<SalonDto>>
{
    private readonly ISalonRepository _repo;
    public ListSalonsByOwnerQueryHandler(ISalonRepository repo) => _repo = repo;

    public async Task<IEnumerable<SalonDto>> Handle(Queries.ListSalonsByOwnerQuery.ListSalonsByOwnerQuery req, CancellationToken ct)
    {
        var salons = await _repo.ListByOwnerAsync(req.OwnerId);
        return salons.Select(s => new SalonDto(s.Id, s.OwnerId, s.Name, s.Description, s.LogoUrl,
            s.Address, s.City, s.State, s.Phone, s.Latitude, s.Longitude, null, s.Active, null, 0, null, null, false, false, false));
    }
}
