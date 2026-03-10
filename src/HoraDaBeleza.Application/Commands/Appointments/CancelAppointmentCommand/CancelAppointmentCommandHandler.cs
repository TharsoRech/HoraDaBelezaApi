using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Appointments.CancelAppointmentCommand;

public class CancelAppointmentCommandHandler(
    IAppointmentRepository repo,
    INotificationRepository notificationRepo,
    IProfessionalRepository professionalRepo,
    IUserRepository userRepo
)
    : IRequestHandler<CancelAppointmentCommand, Unit>
{

    public async Task<Unit> Handle(Appointments.CancelAppointmentCommand.CancelAppointmentCommand req, CancellationToken ct)
    {
        var appointment  = await repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Appointment", req.Id);
        var professional = await professionalRepo.GetByIdAsync(appointment.ProfessionalId);

        bool isClient       = appointment.ClientId == req.UserId;
        bool isProfessional = professional?.UserId == req.UserId;

        if (!isClient && !isProfessional)
            throw new UnauthorizedException();

        if (appointment.Status == AppointmentStatus.Completed)
            throw new BusinessException("Cannot cancel a completed appointment.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new BusinessException("Appointment is already cancelled.");

        await repo.UpdateStatusAsync(req.Id, AppointmentStatus.Cancelled);

        int notifyUserId = isClient ? (professional?.UserId ?? 0) : appointment.ClientId;
        if (notifyUserId > 0)
        {
            var cancelledBy = await userRepo.GetByIdAsync(req.UserId);
            await notificationRepo.CreateAsync(new Notification
            {
                UserId      = notifyUserId,
                Title       = "Appointment cancelled",
                Message     = $"{cancelledBy?.Name} cancelled the appointment for {appointment.ScheduledAt:MM/dd/yyyy HH:mm}.",
                Type        = NotificationType.AppointmentCancelled,
                ReferenceId = req.Id
            });
        }

        return Unit.Value;
    }
}
