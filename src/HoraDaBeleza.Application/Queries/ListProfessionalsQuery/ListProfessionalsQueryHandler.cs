using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListProfessionalsQuery;

public class ListProfessionalsQueryHandler : IRequestHandler<ListProfessionalsQuery, IEnumerable<ProfessionalDto>>
{
    private readonly IProfessionalRepository _repo;
    public ListProfessionalsQueryHandler(IProfessionalRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProfessionalDto>> Handle(Queries.ListProfessionalsQuery.ListProfessionalsQuery req, CancellationToken ct)
    {
        var professionals = await _repo.ListBySalonAsync(req.SalonId);
        return professionals.Select(p => new ProfessionalDto(p.Id, p.UserId, p.SalonId, "",
            null, p.Specialty, p.Bio, p.AverageRating, p.TotalReviews, p.Active));
    }
}
