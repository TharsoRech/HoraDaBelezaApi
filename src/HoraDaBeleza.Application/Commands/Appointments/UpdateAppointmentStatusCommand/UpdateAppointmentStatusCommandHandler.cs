using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Appointments.UpdateAppointmentStatusCommand;

public class UpdateAppointmentStatusCommandHandler(
    IAppointmentRepository repo,
    IProfessionalRepository professionalRepo
) : IRequestHandler<UpdateAppointmentStatusCommand, Unit>
{

    public async Task<Unit> Handle(Appointments.UpdateAppointmentStatusCommand.UpdateAppointmentStatusCommand req, CancellationToken ct)
    {
        var appointment  = await repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Appointment", req.Id);
        var professional = await professionalRepo.GetByIdAsync(appointment.ProfessionalId);

        if (professional?.UserId != req.ProfessionalUserId)
            throw new UnauthorizedException();

        await repo.UpdateStatusAsync(req.Id, req.NewStatus);
        return Unit.Value;
    }
}
