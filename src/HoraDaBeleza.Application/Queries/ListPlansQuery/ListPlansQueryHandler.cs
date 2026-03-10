using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListPlansQuery
{

    public class ListPlansQueryHandler : IRequestHandler<ListPlansQuery, IEnumerable<PlanDto>>
    {
        private readonly IPlanRepository _repo;
        public ListPlansQueryHandler(IPlanRepository repo) => _repo = repo;

        public async Task<IEnumerable<PlanDto>> Handle(Queries.ListPlansQuery.ListPlansQuery req, CancellationToken ct)
        {
            var plans = await _repo.ListActiveAsync();
            return plans.Select(p => new PlanDto(p.Id, p.Name, p.Description, p.Price, p.PeriodDays, p.AppointmentLimit));
        }
    }
}
