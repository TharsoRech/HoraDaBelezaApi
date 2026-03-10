using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using MediatR;

namespace HoraDaBeleza.Application.Queries.Appointments;

public class ListClientAppointmentsQueryHandler : IRequestHandler<ListClientAppointmentsQuery.ListClientAppointmentsQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentRepository _repo;
    public ListClientAppointmentsQueryHandler(IAppointmentRepository repo) => _repo = repo;

    public async Task<IEnumerable<AppointmentDto>> Handle(ListClientAppointmentsQuery.ListClientAppointmentsQuery req, CancellationToken ct)
    {
        var items = await _repo.ListByClientAsync(req.ClientId);
        return items.Select(a => new AppointmentDto(a.Id, a.ClientId, "", a.ProfessionalId, "",
            a.ServiceId, "", a.SalonId, "", a.ScheduledAt, a.DurationMinutes,
            a.TotalPrice, a.Status, a.Notes, a.CreatedAt));
    }
}
