using HoraDaBeleza.Application.DTOs;
using MediatR;

namespace HoraDaBeleza.Application.Commands.Appointments;

public record CreateAppointmentCommand(int ClientId, int ProfessionalId, int ServiceId,
    int SalonId, DateTime ScheduledAt, string? Notes) : IRequest<AppointmentDto>;
