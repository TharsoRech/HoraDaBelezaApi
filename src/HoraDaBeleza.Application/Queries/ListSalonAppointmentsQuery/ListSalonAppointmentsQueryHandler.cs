using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListSalonAppointmentsQuery;

public class ListSalonAppointmentsQueryHandler : IRequestHandler<ListSalonAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentRepository _repo;
    public ListSalonAppointmentsQueryHandler(IAppointmentRepository repo) => _repo = repo;

    public async Task<IEnumerable<AppointmentDto>> Handle(Queries.ListSalonAppointmentsQuery.ListSalonAppointmentsQuery req, CancellationToken ct)
    {
        var items = await _repo.ListBySalonAsync(req.SalonId, req.Date);
        return items.Select(a => new AppointmentDto(a.Id, a.ClientId, "", a.ProfessionalId, "",
            a.ServiceId, "", a.SalonId, "", a.ScheduledAt, a.DurationMinutes,
            a.TotalPrice, a.Status, a.Notes, a.CreatedAt));
    }
}
