using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListServicesQuery;

public class ListServicesQueryHandler : IRequestHandler<ListServicesQuery, IEnumerable<ServiceDto>>
{
    private readonly IServiceRepository _repo;
    public ListServicesQueryHandler(IServiceRepository repo) => _repo = repo;

    public async Task<IEnumerable<ServiceDto>> Handle(Queries.ListServicesQuery.ListServicesQuery req, CancellationToken ct)
    {
        var services = await _repo.ListBySalonAsync(req.SalonId, req.CategoryId);
        return services.Select(s => new ServiceDto(s.Id, s.SalonId, s.CategoryId, "",
            s.Name, s.Description, s.Price, s.DurationMinutes, s.Active));
    }
}
