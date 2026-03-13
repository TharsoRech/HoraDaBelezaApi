using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListPopularSalonsQuery;

public class ListPopularSalonsQueryHandler : IRequestHandler<ListPopularSalonsQuery, IEnumerable<SalonDto>>
{
    private readonly ISalonRepository _repo;
    public ListPopularSalonsQueryHandler(ISalonRepository repo) => _repo = repo;

    public async Task<IEnumerable<SalonDto>> Handle(ListPopularSalonsQuery req, CancellationToken ct)
    {
        var salons = await _repo.GetPopularAsync();
        return salons.Select(s => new SalonDto(
            s.Id, s.OwnerId, s.Name, s.Description, s.LogoUrl,
            s.Address, s.City, s.State, s.Phone, s.Latitude, s.Longitude, 
            null, s.Active, null, 0, null, null, false, false, false));
    }
}