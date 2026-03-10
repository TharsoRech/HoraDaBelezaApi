using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListProfessionalAppointmentsQuery;

public class ListProfessionalAppointmentsQueryHandler : IRequestHandler<ListProfessionalAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentRepository _repo;
    public ListProfessionalAppointmentsQueryHandler(IAppointmentRepository repo) => _repo = repo;

    public async Task<IEnumerable<AppointmentDto>> Handle(Queries.ListProfessionalAppointmentsQuery.ListProfessionalAppointmentsQuery req, CancellationToken ct)
    {
        var items = await _repo.ListByProfessionalAsync(req.ProfessionalId, req.Date);
        return items.Select(a => new AppointmentDto(a.Id, a.ClientId, "", a.ProfessionalId, "",
            a.ServiceId, "", a.SalonId, "", a.ScheduledAt, a.DurationMinutes,
            a.TotalPrice, a.Status, a.Notes, a.CreatedAt));
    }
}
