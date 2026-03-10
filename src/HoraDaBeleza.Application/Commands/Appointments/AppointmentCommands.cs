using FluentValidation;
using HoraDaBeleza.Application.DTOs;
using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Domain.Entities;
using HoraDaBeleza.Domain.Enums;
using HoraDaBeleza.Domain.Exceptions;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Appointments;

// ── Create ─────────────────────────────────────────────────────────────────
public record CreateAppointmentCommand(int ClientId, int ProfessionalId, int ServiceId,
    int SalonId, DateTime ScheduledAt, string? Notes) : IRequest<AppointmentDto>;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.ProfessionalId).GreaterThan(0);
        RuleFor(x => x.ServiceId).GreaterThan(0);
        RuleFor(x => x.SalonId).GreaterThan(0);
        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Appointment date must be in the future.");
    }
}

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    private readonly IAppointmentRepository  _appointmentRepo;
    private readonly IServiceRepository      _serviceRepo;
    private readonly IProfessionalRepository _professionalRepo;
    private readonly ISalonRepository        _salonRepo;
    private readonly IUserRepository         _userRepo;
    private readonly INotificationRepository _notificationRepo;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepo, IServiceRepository serviceRepo,
        IProfessionalRepository professionalRepo, ISalonRepository salonRepo,
        IUserRepository userRepo, INotificationRepository notificationRepo)
    {
        _appointmentRepo  = appointmentRepo;
        _serviceRepo      = serviceRepo;
        _professionalRepo = professionalRepo;
        _salonRepo        = salonRepo;
        _userRepo         = userRepo;
        _notificationRepo = notificationRepo;
    }

    public async Task<AppointmentDto> Handle(CreateAppointmentCommand req, CancellationToken ct)
    {
        var service = await _serviceRepo.GetByIdAsync(req.ServiceId)
            ?? throw new NotFoundException("Service", req.ServiceId);

        if (!service.Active)
            throw new BusinessException("This service is currently unavailable.");

        var professional = await _professionalRepo.GetByIdAsync(req.ProfessionalId)
            ?? throw new NotFoundException("Professional", req.ProfessionalId);

        if (professional.SalonId != req.SalonId)
            throw new BusinessException("Professional does not belong to this salon.");

        if (await _appointmentRepo.HasConflictAsync(req.ProfessionalId, req.ScheduledAt, service.DurationMinutes))
            throw new BusinessException("This time slot is unavailable. Please choose another time.");

        var appointment = new Appointment
        {
            ClientId        = req.ClientId,
            ProfessionalId  = req.ProfessionalId,
            ServiceId       = req.ServiceId,
            SalonId         = req.SalonId,
            ScheduledAt     = req.ScheduledAt,
            DurationMinutes = service.DurationMinutes,
            TotalPrice      = service.Price,
            Status          = AppointmentStatus.Pending,
            Notes           = req.Notes
        };

        var id       = await _appointmentRepo.CreateAsync(appointment);
        var salon    = await _salonRepo.GetByIdAsync(req.SalonId);
        var client   = await _userRepo.GetByIdAsync(req.ClientId);
        var profUser = await _userRepo.GetByIdAsync(professional.UserId);

        if (profUser != null && client != null)
        {
            await _notificationRepo.CreateAsync(new Notification
            {
                UserId      = profUser.Id,
                Title       = "New appointment",
                Message     = $"{client.Name} booked {service.Name} for {req.ScheduledAt:MM/dd/yyyy HH:mm}.",
                Type        = NotificationType.AppointmentConfirmed,
                ReferenceId = id
            });
        }

        return new AppointmentDto(id, req.ClientId, client?.Name ?? "",
            req.ProfessionalId, profUser?.Name ?? "",
            req.ServiceId, service.Name, req.SalonId, salon?.Name ?? "",
            req.ScheduledAt, service.DurationMinutes, service.Price,
            AppointmentStatus.Pending, req.Notes, DateTime.UtcNow);
    }
}

// ── Cancel ─────────────────────────────────────────────────────────────────
public record CancelAppointmentCommand(int Id, int UserId) : IRequest<Unit>;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Unit>
{
    private readonly IAppointmentRepository  _repo;
    private readonly INotificationRepository _notificationRepo;
    private readonly IProfessionalRepository _professionalRepo;
    private readonly IUserRepository         _userRepo;

    public CancelAppointmentCommandHandler(IAppointmentRepository repo,
        INotificationRepository notificationRepo,
        IProfessionalRepository professionalRepo,
        IUserRepository userRepo)
    {
        _repo             = repo;
        _notificationRepo = notificationRepo;
        _professionalRepo = professionalRepo;
        _userRepo         = userRepo;
    }

    public async Task<Unit> Handle(CancelAppointmentCommand req, CancellationToken ct)
    {
        var appointment  = await _repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Appointment", req.Id);
        var professional = await _professionalRepo.GetByIdAsync(appointment.ProfessionalId);

        bool isClient       = appointment.ClientId == req.UserId;
        bool isProfessional = professional?.UserId == req.UserId;

        if (!isClient && !isProfessional)
            throw new UnauthorizedException();

        if (appointment.Status == AppointmentStatus.Completed)
            throw new BusinessException("Cannot cancel a completed appointment.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new BusinessException("Appointment is already cancelled.");

        await _repo.UpdateStatusAsync(req.Id, AppointmentStatus.Cancelled);

        int notifyUserId = isClient ? (professional?.UserId ?? 0) : appointment.ClientId;
        if (notifyUserId > 0)
        {
            var cancelledBy = await _userRepo.GetByIdAsync(req.UserId);
            await _notificationRepo.CreateAsync(new Notification
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

// ── Update Status ──────────────────────────────────────────────────────────
public record UpdateAppointmentStatusCommand(int Id, int ProfessionalUserId,
    AppointmentStatus NewStatus) : IRequest<Unit>;

public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, Unit>
{
    private readonly IAppointmentRepository  _repo;
    private readonly IProfessionalRepository _professionalRepo;

    public UpdateAppointmentStatusCommandHandler(IAppointmentRepository repo,
        IProfessionalRepository professionalRepo)
    {
        _repo             = repo;
        _professionalRepo = professionalRepo;
    }

    public async Task<Unit> Handle(UpdateAppointmentStatusCommand req, CancellationToken ct)
    {
        var appointment  = await _repo.GetByIdAsync(req.Id) ?? throw new NotFoundException("Appointment", req.Id);
        var professional = await _professionalRepo.GetByIdAsync(appointment.ProfessionalId);

        if (professional?.UserId != req.ProfessionalUserId)
            throw new UnauthorizedException();

        await _repo.UpdateStatusAsync(req.Id, req.NewStatus);
        return Unit.Value;
    }
}
