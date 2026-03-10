using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListSalonAppointmentsQuery;

public record ListSalonAppointmentsQuery(int SalonId, DateTime? Date) : IRequest<IEnumerable<AppointmentDto>>;
