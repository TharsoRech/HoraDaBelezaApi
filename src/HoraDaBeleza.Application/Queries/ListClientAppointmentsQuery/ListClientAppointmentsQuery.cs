using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListClientAppointmentsQuery;

public record ListClientAppointmentsQuery(int ClientId) : IRequest<IEnumerable<AppointmentDto>>;
