using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListSalonsQuery;

public class ListSalonsQueryHandler : IRequestHandler<ListSalonsQuery, IEnumerable<SalonDto>>
{
    private readonly ISalonRepository _repo;
    public ListSalonsQueryHandler(ISalonRepository repo) => _repo = repo;

    public async Task<IEnumerable<SalonDto>> Handle(ListSalonsQuery req, CancellationToken ct)
    {
        var salons = await _repo.ListAsync(req.City, req.Search);
        return salons.Select(s => new SalonDto(s.Id, s.OwnerId, s.Name, s.Description, s.LogoUrl,
            s.Address, s.City, s.State, s.Phone, s.Latitude, s.Longitude, null, s.Active, null, 0, null, null, false, false, false));
    }
}
