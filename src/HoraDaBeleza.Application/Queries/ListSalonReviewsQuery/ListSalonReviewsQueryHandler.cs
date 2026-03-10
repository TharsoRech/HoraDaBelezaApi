using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListSalonReviewsQuery;

public class ListSalonReviewsQueryHandler : IRequestHandler<ListSalonReviewsQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _repo;
    public ListSalonReviewsQueryHandler(IReviewRepository repo) => _repo = repo;

    public async Task<IEnumerable<ReviewDto>> Handle(Queries.ListSalonReviewsQuery.ListSalonReviewsQuery req, CancellationToken ct)
    {
        var items = await _repo.ListBySalonAsync(req.SalonId);
        return items.Select(r => new ReviewDto(r.Id, r.AppointmentId, "", r.Rating, r.Comment, r.CreatedAt));
    }
}
