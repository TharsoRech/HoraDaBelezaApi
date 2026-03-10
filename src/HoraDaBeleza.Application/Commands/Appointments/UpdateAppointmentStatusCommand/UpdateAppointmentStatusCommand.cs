using HoraDaBeleza.Domain.Enums;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Appointments.UpdateAppointmentStatusCommand;

public record UpdateAppointmentStatusCommand(int Id, int ProfessionalUserId,
    AppointmentStatus NewStatus) : IRequest<Unit>;
