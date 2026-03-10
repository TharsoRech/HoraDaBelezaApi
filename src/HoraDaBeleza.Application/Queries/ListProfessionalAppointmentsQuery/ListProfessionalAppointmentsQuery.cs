using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Queries.ListProfessionalAppointmentsQuery;

public record ListProfessionalAppointmentsQuery(int ProfessionalId, DateTime? Date) : IRequest<IEnumerable<AppointmentDto>>;
