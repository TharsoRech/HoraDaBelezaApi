using MediatR;

namespace HoraDaBeleza.Application.Commands.Appointments.CancelAppointmentCommand;

public record CancelAppointmentCommand(int Id, int UserId) : IRequest<Unit>;
